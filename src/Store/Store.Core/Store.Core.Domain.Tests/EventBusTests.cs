using System;
using System.Threading.Tasks;
using Store.Core.Domain.Event;
using Store.Core.Domain.Event.InMemory;
using Xunit;

namespace Store.Core.Domain.Tests
{
    public class EventBusTests
    {
        private class TestIntegrationEventSubscriber : EventSubscriberBase<TestIntegrationEvent>
        {
            public bool HandleEventRan { get; private set; }
            
            public TestIntegrationEventSubscriber(IEventBus bus) : base(bus)
            {
            }

            public override Task HandleEvent(TestIntegrationEvent eventData)
            {
                if (eventData == null) throw new ArgumentNullException(nameof(eventData));

                HandleEventRan = true;
                
                return Task.CompletedTask;
            }
        }

        private class TestIntegrationEvent : IIntegrationEvent
        {
            public string TestValue { get; set; }
        }
        
        [Fact]
        public void Should_RegisterSubscriber()
        {
            IEventBus bus = new InMemoryEventBus();
            IEventSubscriber<TestIntegrationEvent> subscriber = new TestIntegrationEventSubscriber(bus);
        }

        [Fact]
        public void Should_SendEvent_To_RegisteredSubscriber()
        {
            IEventBus bus = new InMemoryEventBus();
            IEventSubscriber<TestIntegrationEvent> subscriber = new TestIntegrationEventSubscriber(bus);

            bus.PublishAsync(new TestIntegrationEvent { TestValue = "TestValue" });
            
            Assert.True(((TestIntegrationEventSubscriber)subscriber).HandleEventRan);
        }
    }
}