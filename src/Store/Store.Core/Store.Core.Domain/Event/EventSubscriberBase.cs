using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public abstract class EventSubscriberBase<T> : IEventSubscriber<T> where T : IIntegrationEvent
    {
        protected EventSubscriberBase(IEventBus bus)
        {
            bus.SubscribeAsync<T, IEventSubscriber<T>>(this).GetAwaiter().GetResult();
        }

        public abstract Task HandleEvent(T eventData);
    }
}