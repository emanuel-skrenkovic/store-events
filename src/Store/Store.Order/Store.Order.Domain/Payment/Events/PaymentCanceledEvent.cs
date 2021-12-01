using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Payment.Events
{
    public record PaymentCanceledEvent(Guid PaymentId, PaymentStatus Status) : IEvent;
}