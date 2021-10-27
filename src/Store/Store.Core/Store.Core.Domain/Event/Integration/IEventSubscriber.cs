using System.Threading.Tasks;

namespace Store.Core.Domain.Event.Integration
{
    public interface IEventSubscriber<T> where T : IEvent
    {
        Task HandleEvent(T eventData);
    }
}