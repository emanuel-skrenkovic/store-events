using System;

namespace Store.Core.Domain.Event
{
    public interface IEvent
    {
        Guid EntityId { get; }
    }
}