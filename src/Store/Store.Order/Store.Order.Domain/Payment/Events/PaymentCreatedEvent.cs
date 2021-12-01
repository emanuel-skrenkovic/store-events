using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Payment.Events
{
    public record PaymentCreatedEvent(
        Guid PaymentId, 
        string CustomerNumber, 
        Guid OrderId,
        decimal Amount,
        PaymentStatus Status) : IEvent;
}