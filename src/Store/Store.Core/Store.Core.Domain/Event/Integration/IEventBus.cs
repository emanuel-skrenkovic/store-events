using System.Threading.Tasks;

namespace Store.Core.Domain.Event.Integration
{
    // TODO: don't use IIntegration event here. Needs to work with domain events as well.
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}