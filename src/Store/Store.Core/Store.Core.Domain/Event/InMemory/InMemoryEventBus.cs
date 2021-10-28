using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Store.Core.Domain.Event.Integration;

namespace Store.Core.Domain.Event.InMemory
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly IReadOnlyCollection<IEventSubscriber> _subscribers;

        public InMemoryEventBus(IEnumerable<IEventSubscriber> subscribers)
        {
            _subscribers = subscribers?.ToImmutableList() ?? throw new ArgumentNullException(nameof(subscribers));
        }

        public Task PublishAsync<TEvent>(TEvent integrationEvent) where TEvent : IEvent
        {
            if (_subscribers?.Any() != true) return Task.CompletedTask;

            foreach (IEventSubscriber sub in _subscribers.Where(s => s.Handles(typeof(TEvent))))
            {
                try
                {
                    sub.Handle(integrationEvent);
                }
                catch
                {
                    // Need to publish to all subscribers regardless if some of them break.
                    // TODO: add logging here or create logging wrapper.
                }
            }

            return Task.CompletedTask;
        }
    }
}