using System;
using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventBus _bus;

        public EventPublisher(IEventBus bus)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }
        
        public Task PublishAsync(IIntegrationEvent integrationEvent)
        {
            return _bus.PublishAsync(integrationEvent);
        }
    }
}