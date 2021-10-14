using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Store.Core.Domain
{
    public abstract class AggregateEntity
    {
        public Guid Id { get; set; }

        protected readonly ICollection<IEvent> _events;
        
        public abstract void ApplyEvent(IEvent domainEvent);

        public IReadOnlyCollection<IEvent> GetUncommittedEvents() => _events.ToImmutableList();
    }
}