using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Core.Domain.Event.InMemory
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly IEventSubscriberProvider _eventSubscriberProvider;

        public InMemoryEventBus(IEventSubscriberProvider eventSubscriberProvider)
        {
            _eventSubscriberProvider = eventSubscriberProvider 
                                       ?? throw new ArgumentNullException(nameof(eventSubscriberProvider));
        }

        public Task PublishAsync<TEvent>(TEvent integrationEvent) where TEvent : IIntegrationEvent
        {
            IEnumerable<IEventSubscriber<TEvent>> subscribers = _eventSubscriberProvider
                .GetSubscribers<TEvent>()
                .ToImmutableArray();
            
            if (!subscribers.Any()) return Task.CompletedTask;

            foreach (IEventSubscriber<TEvent> sub in subscribers)
            {
                sub.HandleEvent(integrationEvent);
            }

            return Task.CompletedTask;
        }
    }
}