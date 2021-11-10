using System;
using System.Threading.Tasks;
using Store.Core.Domain.Event;

namespace Store.Core.Domain.Projection
{
    public interface IProjection<T, TContext>
    { 
        Func<Task> Project(IEvent receivedEvent, TContext context);
    }
}