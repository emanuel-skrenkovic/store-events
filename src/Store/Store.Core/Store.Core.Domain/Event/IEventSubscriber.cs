using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public interface IEventSubscriber<T> where T : IIntegrationEvent
    {
        Task HandleEvent(T eventData);
    }
}