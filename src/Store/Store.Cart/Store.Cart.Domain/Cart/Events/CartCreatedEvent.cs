using System;
using Store.Core.Domain.Event;

namespace Store.Cart.Domain.Events
{
    public record CartCreatedEvent(Guid EntityId, Guid CustomerId) : IEvent;
}