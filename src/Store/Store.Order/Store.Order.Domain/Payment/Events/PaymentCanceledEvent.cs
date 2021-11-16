using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Payment.Events
{
    public record PaymentCanceledEvent(Guid EntityId) : IEvent;
}