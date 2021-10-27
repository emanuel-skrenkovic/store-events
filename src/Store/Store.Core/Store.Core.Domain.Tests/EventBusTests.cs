using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store.Core.Domain.Event;
using Store.Core.Domain.Event.InMemory;
using Store.Core.Domain.Event.Integration;
using Xunit;

namespace Store.Core.Domain.Tests
{
    public class EventBusTests
    {
        [Fact]
        public void Should_SendEvent_To_Subscriber()
        {
            IEventSubscriber<TestIntegrationEvent> subscriber = new TestIntegrationEventSubscriber();
            IEventSubscriberProvider provider = new InMemoryEventSubscriberProvider(new[] { subscriber });

            IEventBus bus = new InMemoryEventBus(provider);
            bus.PublishAsync(new TestIntegrationEvent { TestValue = "TestValue" });
            
            Assert.True(((TestIntegrationEventSubscriber)subscriber).HandleEventRan);
        }
        
        [Fact]
        public void Should_SendEvent_To_MultipleSubscribers()
        {
            IEventSubscriber<TestIntegrationEvent> subscriber = new TestIntegrationEventSubscriber();
            IEventSubscriber<TestIntegrationEvent> subscriber2 = new TestIntegrationEventSubscriber2();
            IEventSubscriberProvider provider = new InMemoryEventSubscriberProvider(new[] { subscriber, subscriber2 });

            IEventBus bus = new InMemoryEventBus(provider);
            bus.PublishAsync(new TestIntegrationEvent { TestValue = "TestValue" });
            
            Assert.True(((TestIntegrationEventSubscriber)subscriber).HandleEventRan);
            Assert.True(((TestIntegrationEventSubscriber2)subscriber2).HandleEventRan);
        }
        
        [Fact]
        public void Should_Not_SendEvent_To_NonSubscribers()
        {
            IEventSubscriber<TestIntegrationEvent> subscriber = new TestIntegrationEventSubscriber();
            IEventSubscriberProvider provider = new InMemoryEventSubscriberProvider(new[] { subscriber });

            IEventBus bus = new InMemoryEventBus(provider);
            bus.PublishAsync(new TestIntegrationEvent2 { TestValue = 2 });
            
            Assert.False(((TestIntegrationEventSubscriber)subscriber).HandleEventRan);
        }
        
        [Fact]
        public void Should_SendEvent_Only_To_Subscribers()
        {
            IEventSubscriber<TestIntegrationEvent> subscriber = new TestIntegrationEventSubscriber();
            IEventSubscriber<TestIntegrationEvent> subscriber2 = new TestIntegrationEventSubscriber2();
            IEventSubscriber<TestIntegrationEvent2> subscriber3 = new TestIntegrationEventSubscriber3();
            
            IEventSubscriberProvider provider = new InMemoryEventSubscriberProvider(new dynamic[]
            {
                subscriber, subscriber2, subscriber3
            });

            IEventBus bus = new InMemoryEventBus(provider);
            bus.PublishAsync(new TestIntegrationEvent { TestValue = "TestValue" });
            
            Assert.True(((TestIntegrationEventSubscriber)subscriber).HandleEventRan);
            Assert.True(((TestIntegrationEventSubscriber2)subscriber2).HandleEventRan);
            Assert.False(((TestIntegrationEventSubscriber3)subscriber3).HandleEventRan);
        }
        
        private class InMemoryEventSubscriberProvider : IEventSubscriberProvider
        {
            private static readonly ConcurrentDictionary<Type, List<object>> Subscribers = new();
            
            public InMemoryEventSubscriberProvider(IEnumerable<object> subscribers)
            {
                foreach (object sub in subscribers)
                {
                    Type eventType = sub.GetType().GetInterfaces()[0].GetGenericArguments()[0];
                    
                    if (!Subscribers.ContainsKey(eventType))
                    {
                        Subscribers.TryAdd(eventType, new List<object>());
                    }
                    
                    Subscribers[eventType].Add(sub);
                }
            }
            
            public IEnumerable<IEventSubscriber<TEvent>> GetSubscribers<TEvent>() where TEvent : IEvent 
            {
                if (!Subscribers.ContainsKey(typeof(TEvent))) return new IEventSubscriber<TEvent>[] { };
                
                return Subscribers[typeof(TEvent)].Select(sub => (IEventSubscriber<TEvent>)sub);
            }
        }
        
        private class TestIntegrationEventSubscriber : IEventSubscriber<TestIntegrationEvent>
        {
            public bool HandleEventRan { get; private set; }
            
            public Task HandleEvent(TestIntegrationEvent eventData)
            {
                if (eventData == null) throw new ArgumentNullException(nameof(eventData));

                HandleEventRan = true;
                
                return Task.CompletedTask;
            }
        }
        
        private class TestIntegrationEventSubscriber2 : IEventSubscriber<TestIntegrationEvent>
        {
            public bool HandleEventRan { get; private set; }
            
            public Task HandleEvent(TestIntegrationEvent eventData)
            {
                if (eventData == null) throw new ArgumentNullException(nameof(eventData));

                HandleEventRan = true;
                
                return Task.CompletedTask;
            }
        }
        
        private class TestIntegrationEventSubscriber3 : IEventSubscriber<TestIntegrationEvent2>
        {
            public bool HandleEventRan { get; private set; }
            
            public Task HandleEvent(TestIntegrationEvent2 eventData)
            {
                if (eventData == null) throw new ArgumentNullException(nameof(eventData));

                HandleEventRan = true;
                
                return Task.CompletedTask;
            }
        }

        private class TestIntegrationEvent :IEvent 
        {
            public string TestValue { get; set; }
        }

        private class TestIntegrationEvent2 : IEvent
        {
            public int TestValue { get; set; }
        }
    }
}