using System;
using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public interface IEventSubscriber
    {
        Task Handle(IEvent @event);
        
        bool Handles(Type type);
    }
}