using System;
using System.Threading.Tasks;
using Store.Core.Domain.Event;

namespace Store.Core.Domain.Projection
{
    public interface IProjection<T>
    { 
        Func<Task> Project(IEvent receivedEvent);
    }
}