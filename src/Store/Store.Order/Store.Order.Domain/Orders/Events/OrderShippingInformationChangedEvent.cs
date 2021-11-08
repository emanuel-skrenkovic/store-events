using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Orders.Events
{
    public record OrderShippingInformationChangedEvent(Guid EntityId, ShippingInformation ShippingInformation) : IEvent;
}