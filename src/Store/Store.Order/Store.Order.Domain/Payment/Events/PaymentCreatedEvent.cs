using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Payment.Events
{
    public record PaymentCreatedEvent(
        Guid EntityId, 
        string PaymentNumber, 
        string CustomerNumber, 
        string OrderNumber,
        decimal Amount,
        PaymentStatus Status) : IEvent;
}