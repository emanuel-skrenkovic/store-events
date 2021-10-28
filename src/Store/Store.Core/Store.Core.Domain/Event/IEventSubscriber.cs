using System;
using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public interface IEventSubscriber
    {
        Task Handle(object @event);
        
        bool Handles(Type type);
    }
}