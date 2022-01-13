using System;
using System.Collections.Generic;
using Store.Core.Domain.Event;
using Store.Shopping.Domain.Orders.ValueObjects;

namespace Store.Shopping.Domain.Orders.Events;

public record OrderConfirmedEvent(Guid OrderId, OrderStatus Status) : IEvent;