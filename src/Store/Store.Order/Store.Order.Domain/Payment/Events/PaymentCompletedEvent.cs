using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Payment.Events
{
    public record PaymentCompletedEvent(Guid PaymentId, PaymentStatus Status) : IEvent;
}