using System;
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
            IEventSubscriber subscriber = new TestIntegrationEventSubscriber();

            IEventBus bus = new InMemoryEventBus(new[] { subscriber });
            bus.PublishAsync(new TestIntegrationEvent { TestValue = "TestValue" });
            
            Assert.True(((TestIntegrationEventSubscriber)subscriber).HandleEventRan);
        }
        
        [Fact]
        public void Should_SendEvent_To_MultipleSubscribers()
        {
            IEventSubscriber subscriber = new TestIntegrationEventSubscriber();
            IEventSubscriber subscriber2 = new TestIntegrationEventSubscriber2();

            IEventBus bus = new InMemoryEventBus(new[] { subscriber, subscriber2 });
            bus.PublishAsync(new TestIntegrationEvent { TestValue = "TestValue" });
            
            Assert.True(((TestIntegrationEventSubscriber)subscriber).HandleEventRan);
            Assert.True(((TestIntegrationEventSubscriber2)subscriber2).HandleEventRan);
        }
        
        [Fact]
        public void Should_Not_SendEvent_To_NonSubscribers()
        {
            IEventSubscriber subscriber = new TestIntegrationEventSubscriber();

            IEventBus bus = new InMemoryEventBus(new[] { subscriber });
            bus.PublishAsync(new TestIntegrationEvent2 { TestValue = 2 });
            
            Assert.False(((TestIntegrationEventSubscriber)subscriber).HandleEventRan);
        }
        
        [Fact]
        public void Should_SendEvent_Only_To_Subscribers()
        {
            IEventSubscriber subscriber = new TestIntegrationEventSubscriber();
            IEventSubscriber subscriber2 = new TestIntegrationEventSubscriber2();
            IEventSubscriber subscriber3 = new TestIntegrationEventSubscriber3();

            IEventBus bus = new InMemoryEventBus(new[] { subscriber, subscriber2, subscriber3 });
            bus.PublishAsync(new TestIntegrationEvent { TestValue = "TestValue" });
            
            Assert.True(((TestIntegrationEventSubscriber)subscriber).HandleEventRan);
            Assert.True(((TestIntegrationEventSubscriber2)subscriber2).HandleEventRan);
            Assert.False(((TestIntegrationEventSubscriber3)subscriber3).HandleEventRan);
        }
        
        private class TestIntegrationEventSubscriber : IEventSubscriber
        {
            public bool HandleEventRan { get; private set; }

            public Task Handle(object @event)
            {
                if (@event == null) throw new ArgumentNullException(nameof(@event));

                HandleEventRan = true;
                
                return Task.CompletedTask;
            }

            public bool Handles(Type type) => type == typeof(TestIntegrationEvent);
        }
        
        private class TestIntegrationEventSubscriber2 : IEventSubscriber
        {
            public bool HandleEventRan { get; private set; }
            
            public Task Handle(object @event)
            {
                if (@event == null) throw new ArgumentNullException(nameof(@event));

                HandleEventRan = true;
                
                return Task.CompletedTask;
            }

            public bool Handles(Type type) => type == typeof(TestIntegrationEvent);
        }
        
        private class TestIntegrationEventSubscriber3 : IEventSubscriber
        {
            public bool HandleEventRan { get; private set; }
            
            public Task Handle(object @event)
            {
                if (@event == null) throw new ArgumentNullException(nameof(@event));

                HandleEventRan = true;
                
                return Task.CompletedTask;
            }

            public bool Handles(Type type) => type == typeof(TestIntegrationEvent2);
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