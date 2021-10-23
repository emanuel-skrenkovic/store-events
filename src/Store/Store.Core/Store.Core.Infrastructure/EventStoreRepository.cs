using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.Extensions;

namespace Store.Core.Infrastructure
{
    public class EventStoreRepository : IRepository
    {
        private readonly EventStoreClient _eventStore;
       
        public EventStoreRepository(EventStoreClient eventStore)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }
        
        public async Task<T> GetAsync<T>(Guid id) where T : AggregateEntity, new()
        {
            EventStoreClient.ReadStreamResult eventStream = _eventStore.ReadStreamAsync(
                Direction.Forwards,
                GenerateStreamName<T>(id),
                StreamPosition.Start);
            
            T entity = new();
            
            // TODO: .ToListAsync()?
            await foreach (ResolvedEvent resolvedEvent in eventStream)
            {
                IEvent domainEvent = resolvedEvent.Deserialize() as IEvent;
                entity.ApplyEvent(domainEvent);
            }

            return entity;
        }

        public Task SaveAsync<T>(T entity) where T : AggregateEntity
        {
            Guard.IsNotNull(entity, nameof(entity));

            IReadOnlyCollection<EventData> eventsData = entity.GetUncommittedEvents()
                .Select(domainEvent => domainEvent.ToEventData())
                .ToImmutableList();

            return _eventStore.AppendToStreamAsync(
                GenerateStreamName<T>(Guid.NewGuid()),
                StreamState.Any, 
                eventsData);
        }

        private string GenerateStreamName<T>(Guid id)
        {
            return $"{typeof(T).FullName}-{id}";
        }
    }
}