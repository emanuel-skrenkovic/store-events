using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Orders.Events
{
    public record OrderShippingInformationAddedEvent(Guid EntityId, ShippingInformation ShippingInformation) : IEvent;
}