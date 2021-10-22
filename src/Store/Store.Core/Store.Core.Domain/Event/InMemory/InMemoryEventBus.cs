using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Core.Domain.Event.InMemory
{
    public class InMemoryEventBus : IEventBus
    {
        // List of subscribers should probably only contain unique instance per type.
        private static readonly ConcurrentDictionary<Type, List<object>> Subscribers = new();

        public Task PublishAsync<TEvent>(TEvent integrationEvent) where TEvent : IIntegrationEvent
        {
            if (!Subscribers.ContainsKey(typeof(TEvent))) return Task.CompletedTask;
            
            IReadOnlyCollection<IEventSubscriber<TEvent>> subscribers = Subscribers[typeof(TEvent)]
                .Select(sub => (IEventSubscriber<TEvent>)sub)
                .ToImmutableArray();

            if (!subscribers.Any()) return Task.CompletedTask;

            foreach (IEventSubscriber<TEvent> sub in subscribers)
            {
                sub.HandleEvent(integrationEvent);
            }

            return Task.CompletedTask;
        }

        public Task SubscribeAsync<TEvent, TEventSubscriber>(TEventSubscriber subscriber) 
            where TEvent : IIntegrationEvent
            where TEventSubscriber : IEventSubscriber<TEvent>
        {
            List<object> eventSubscribers = Subscribers.GetOrAdd(typeof(TEvent), _ => new List<object>());
            eventSubscribers.Add(subscriber);

            return Task.CompletedTask;
        }
    }
}