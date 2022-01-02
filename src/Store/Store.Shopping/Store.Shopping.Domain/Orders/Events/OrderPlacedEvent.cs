using System;
using System.Collections.Generic;
using Store.Core.Domain.Event;
using Store.Shopping.Domain.Orders.ValueObjects;

namespace Store.Shopping.Domain.Orders.Events;

public record OrderPlacedEvent(
    Guid OrderId, 
    string CustomerNumber, 
    Guid PaymentId, 
    IReadOnlyCollection<OrderLine> OrderLines, 
    ShippingInfo ShippingInfo) : IEvent;