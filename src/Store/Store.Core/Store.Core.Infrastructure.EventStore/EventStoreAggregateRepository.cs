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
        
        public async Task<T> GetAsync<T>(Guid id) where T : AggregateEntity, new()
        {
            EventStoreClient.ReadStreamResult eventStream = _eventStore.ReadStreamAsync(
                Direction.Forwards,
                GenerateStreamName<T>(id),
                StreamPosition.Start);
            
            IReadOnlyCollection<IEvent> domainEvents = await eventStream
                .Select(e => e.Deserialize(_serializer) as IEvent)
                .ToArrayAsync();

            T entity = new();
            entity.Hydrate(id, domainEvents);

            return entity;
        }

        public Task CreateAsync<T>(T entity) where T : AggregateEntity
        {
            entity.Id = Guid.NewGuid();
            return SaveAsync(entity);
        }

        public Task SaveAsync<T>(T entity) where T : AggregateEntity
        {
            Guard.IsNotNull(entity, nameof(entity));

            IReadOnlyCollection<EventData> eventsData = entity.GetUncommittedEvents()
                .Select(domainEvent => domainEvent.ToEventData(_serializer))
                .ToImmutableList();

            return _eventStore.AppendToStreamAsync(
                GenerateStreamName<T>(entity.Id),
                StreamState.Any, 
                eventsData);
        }

        private string GenerateStreamName<T>(Guid id)
        {
            return $"{typeof(T).FullName}-{id}";
        }
    }
}