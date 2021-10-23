using Store.Core.Domain.Event;
using Xunit;

namespace Store.Core.Domain.Tests
{
    public record CreateTestEntityEvent(string Value1, int? Value2) : IEvent;

    public record Value1SetEvent(string Value1) : IEvent;

    public record Value2SetEvent(int Value2) : IEvent;
    
    public class TestEntity : AggregateEntity
    {
        public string TestValue { get; private set; }
        
        public int TestValue2 { get; private set; }

        public void SetValue1(Value1SetEvent domainEvent)
        {
            ApplyEvent(domainEvent);
        }

        private void Apply(Value1SetEvent domainEvent)
        {
            TestValue = domainEvent.Value1;
        }
        
        public void SetValue2(Value2SetEvent domainEvent)
        {
            ApplyEvent(domainEvent);
        }
        
        private void Apply(Value2SetEvent domainEvent)
        {
            TestValue2 = domainEvent.Value2;
        }

        protected override void RegisterAppliers()
        {
            RegisterApplier<Value1SetEvent>(Apply);
            RegisterApplier<Value2SetEvent>(Apply);
        }
    }
    
    public class AggregateEntityTests
    {
        [Fact]
        public void AggregateEntity_ShouldApplyChange_WithValidEvents()
        {
            TestEntity entity = new TestEntity();
            Value1SetEvent event1 = new Value1SetEvent("test");
            entity.SetValue1(event1);
            
            Assert.Equal(entity.TestValue, event1.Value1);
            Assert.Contains(entity.GetUncommittedEvents(), e => e.Equals(event1));
            
            Value2SetEvent event2 = new Value2SetEvent(3);
            entity.SetValue2(event2);
            
            Assert.Equal(entity.TestValue2, event2.Value2);
            Assert.Contains(entity.GetUncommittedEvents(), e => e.Equals(event2));
        }
    }
}