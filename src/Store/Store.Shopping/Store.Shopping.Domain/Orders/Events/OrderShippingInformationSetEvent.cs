using System;
using Store.Core.Domain.Event;
using Store.Shopping.Domain.Orders.ValueObjects;

namespace Store.Shopping.Domain.Orders.Events;

public record OrderShippingInformationSetEvent(Guid OrderId, ShippingInfo ShippingInfo) : IEvent;