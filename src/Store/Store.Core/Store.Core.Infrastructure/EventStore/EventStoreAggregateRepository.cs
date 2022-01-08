using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EventStore.Extensions;

namespace Store.Core.Infrastructure.EventStore;

public class EventStoreAggregateRepository : IAggregateRepository
{
    private readonly ISerializer _serializer;
    private readonly EventStoreClient _eventStore;
       
    public EventStoreAggregateRepository(ISerializer serializer, EventStoreClient eventStore)
    {
        _serializer = Ensure.NotNull(serializer);
        _eventStore = Ensure.NotNull(eventStore);
    }
        
    public async Task<Result<T>> GetAsync<T, TKey>(TKey id) 
        where T : AggregateEntity<TKey>, new()
    {
        try
        {
            EventStoreClient.ReadStreamResult eventStream = _eventStore.ReadStreamAsync(
                Direction.Forwards,
                GenerateStreamName<T, TKey>(id),
                StreamPosition.Start);

            if (await eventStream.ReadState == ReadState.StreamNotFound)
            {
                return new NotFoundError($"Event stream {id} not found.");
            }

            IReadOnlyCollection<IEvent> domainEvents = await eventStream
                .Select(e => e.Deserialize(_serializer) as IEvent)
                .ToArrayAsync();

            T entity = new();
            entity.Hydrate(id, domainEvents);

            return entity;
        }
        catch (Exception ex)
        {
            return new InternalError(ex.Message, ex.StackTrace);
        }
    }

    public async Task<Result> SaveAsync<T, TKey>(T entity) 
        where T : AggregateEntity<TKey>
    {
        try
        {
            Ensure.NotNull(entity);

            IReadOnlyCollection<EventData> eventsData = entity.GetUncommittedEvents()
                .Select(domainEvent => domainEvent.ToEventData(
                    CorrelationContext.CreateEventMetadata(domainEvent), 
                    _serializer))
                .ToImmutableList();

            await _eventStore.AppendToStreamAsync(
                GenerateStreamName<T, TKey>(entity.Id),
                StreamState.Any, // TODO: fix this
                eventsData);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return new InternalError(ex.Message, ex.StackTrace);
        }
    }

    private string GenerateStreamName<T, TKey>(TKey id)
    {
        return $"{typeof(T).FullName}-{id}";
    }
}