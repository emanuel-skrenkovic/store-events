using System;
using System.Threading.Tasks;

namespace Store.Core.Domain.Event;

public interface IEventDispatcher
{
    Task DispatchAsync(object @event, Guid correlationId, Guid causationId);
}