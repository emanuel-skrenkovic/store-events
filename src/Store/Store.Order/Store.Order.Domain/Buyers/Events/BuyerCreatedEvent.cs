using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Buyers.Events
{
    public record BuyerCreatedEvent(Guid EntityId, CustomerNumber CustomerNumber) : IEvent;
}