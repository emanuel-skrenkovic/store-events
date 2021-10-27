using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public interface IEventSubscriber
    {
        bool Handles(string eventType);
        
        Task Handle(object domainEvent);
    }
}