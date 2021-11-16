using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Orders.Events
{
    public record OrderPaymentDetailsAdded(Guid EntityId, PaymentDetails PaymentDetails) : IEvent;
}