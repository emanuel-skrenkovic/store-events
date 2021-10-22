using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent integrationEvent) where TEvent : IIntegrationEvent;
        
        Task SubscribeAsync<TEvent, TEventSubscriber>(TEventSubscriber subscriber) 
            where TEvent : IIntegrationEvent
            where TEventSubscriber : IEventSubscriber<TEvent>;
    }
}