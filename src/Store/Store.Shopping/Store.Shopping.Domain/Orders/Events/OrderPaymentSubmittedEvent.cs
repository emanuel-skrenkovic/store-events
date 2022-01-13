using System;
using Store.Core.Domain.Event;

namespace Store.Shopping.Domain.Orders.Events;

public record OrderPaymentSubmittedEvent(Guid OrderId, Guid PaymentId) : IEvent;