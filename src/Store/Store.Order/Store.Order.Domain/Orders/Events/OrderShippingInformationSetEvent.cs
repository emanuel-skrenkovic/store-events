using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Orders.Events;

public record OrderShippingInformationSetEvent(Guid OrderId, ShippingInformation ShippingInformation) : IEvent;