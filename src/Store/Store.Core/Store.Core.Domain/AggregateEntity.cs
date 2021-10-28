using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Store.Core.Domain.Event;

namespace Store.Core.Domain
{
    public abstract class AggregateEntity
    {
        private readonly Dictionary<Type, Action<IEvent>> _eventAppliers;

        private readonly ICollection<IEvent> _events;
        
        public Guid Id { get; set; }

        public int Version { get; private set; }
        
        public AggregateEntity(Guid id) : this() { }

        protected AggregateEntity()
        {
            _events = new List<IEvent>();
            _eventAppliers = new();
            
            RegisterAppliers();
        }
        
        public void ApplyEvent(IEvent domainEvent)
        {
            _eventAppliers[domainEvent.GetType()](domainEvent);
            _events.Add(domainEvent);
        }

        public void Hydrate(Guid id, IReadOnlyCollection<IEvent> domainEvents)
        {
            if (domainEvents?.Any() != true) return;

            Id = id;
            
            foreach (IEvent domainEvent in domainEvents)
            {
                ApplyEvent(domainEvent);
                Version++;
            }

            // TODO: check if correct
            _events.Clear();
        }

        // TODO: should probably dequeue, i.e., clear the list.
        public IReadOnlyCollection<IEvent> GetUncommittedEvents() => _events.ToImmutableList();

        protected abstract void RegisterAppliers();
        
        protected void RegisterApplier<TEvent>(Action<TEvent> applier) where TEvent: IEvent
        {
            _eventAppliers.Add(typeof(TEvent), e => applier((TEvent)e));
        }
    }
}