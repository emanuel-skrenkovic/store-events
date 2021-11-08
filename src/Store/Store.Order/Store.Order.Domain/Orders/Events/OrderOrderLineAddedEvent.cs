using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Orders.Events
{
    public record OrderOrderLineAddedEvent(Guid EntityId, OrderLine OrderLine) : IEvent;
}