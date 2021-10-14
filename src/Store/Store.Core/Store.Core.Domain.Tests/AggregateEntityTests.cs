using Xunit;

namespace Store.Core.Domain.Tests
{
    public record CreateTestEntityEvent(string Value1, int? Value2) : IEvent;

    public record SetValue1Event(string Value1) : IEvent;

    public record SetValue2Event(int Value2) : IEvent;
    
    public class TestEntity : AggregateEntity
    {
        public string TestValue { get; private set; }
        
        public int TestValue2 { get; private set; }

        public void SetValue1(SetValue1Event domainEvent)
        {
            ApplyEvent(domainEvent);
        }

        private void Apply(SetValue1Event domainEvent)
        {
            TestValue = domainEvent.Value1;
        }
        
        public void SetValue2(SetValue2Event domainEvent)
        {
            ApplyEvent(domainEvent);
        }
        
        private void Apply(SetValue2Event domainEvent)
        {
            TestValue2 = domainEvent.Value2;
        }

        protected override void RegisterAppliers()
        {
            RegisterApplier<SetValue1Event>(Apply);
            RegisterApplier<SetValue2Event>(Apply);
        }
    }
    
    public class AggregateEntityTests
    {
        [Fact]
        public void AggregateEntity_ShouldApplyChange_WithValidEvent()
        {
            TestEntity entity = new TestEntity();
            SetValue1Event event1 = new SetValue1Event("test");
            entity.SetValue1(event1);
            
            Assert.Equal(entity.TestValue, event1.Value1);
            Assert.Contains(entity.GetUncommittedEvents(), e => e == event1);
        }
    }
}