using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EventStore.Extensions;

namespace Store.Core.Infrastructure.EventStore;

public class EventStoreAggregateRepository : IAggregateRepository
{
    private readonly ISerializer _serializer;
    private readonly EventStoreClient _eventStore;
       
    public EventStoreAggregateRepository(ISerializer serializer, EventStoreClient eventStore)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
    }
        
    public async Task<T> GetAsync<T, TKey>(TKey id) 
        where T : AggregateEntity<TKey>, new()
    {
        EventStoreClient.ReadStreamResult eventStream = _eventStore.ReadStreamAsync(
            Direction.Forwards,
            GenerateStreamName<T, TKey>(id),
            StreamPosition.Start);

        if (await eventStream.ReadState == ReadState.StreamNotFound)
        {
            return null;
        }
            
        IReadOnlyCollection<IEvent> domainEvents = await eventStream
            .Select(e => e.Deserialize(_serializer) as IEvent)
            .ToArrayAsync();

        T entity = new();
        entity.Hydrate(id, domainEvents);

        return entity;
    }

    public Task SaveAsync<T, TKey>(T entity) 
        where T : AggregateEntity<TKey>
    {
        Ensure.NotNull(entity, nameof(entity));

        IReadOnlyCollection<EventData> eventsData = entity.GetUncommittedEvents()
            .Select(domainEvent => domainEvent.ToEventData(_serializer))
            .ToImmutableList();

        return _eventStore.AppendToStreamAsync(
            GenerateStreamName<T, TKey>(entity.Id),
            StreamState.Any, // TODO: fix this
            eventsData);
    }

    private string GenerateStreamName<T, TKey>(TKey id)
    {
        return $"{typeof(T).FullName}-{id}";
    }
}