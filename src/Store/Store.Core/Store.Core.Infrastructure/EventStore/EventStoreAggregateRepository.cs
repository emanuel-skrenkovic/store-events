using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EventStore.Extensions;

namespace Store.Core.Infrastructure.EventStore
{
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
            where TKey : struct
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

        public Task CreateAsync<T, TKey>(T entity) 
            where T : AggregateEntity<TKey> 
            where TKey : struct
        {
            return SaveAsync<T, TKey>(entity);
        }

        public Task SaveAsync<T, TKey>(T entity) 
            where T : AggregateEntity<TKey>
            where TKey : struct
        {
            Guard.IsNotNull(entity, nameof(entity));

            IReadOnlyCollection<EventData> eventsData = entity.GetUncommittedEvents()
                .Select(domainEvent => domainEvent.ToEventData(_serializer))
                .ToImmutableList();

            return _eventStore.AppendToStreamAsync(
                GenerateStreamName<T, TKey>(entity.Id),
                StreamState.Any, 
                eventsData);
        }

        private string GenerateStreamName<T, TKey>(TKey id) where TKey : struct
        {
            return $"{typeof(T).FullName}-{id}";
        }
    }
}