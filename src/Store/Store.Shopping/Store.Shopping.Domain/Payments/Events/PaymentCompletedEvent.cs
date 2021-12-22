using System;
using Store.Core.Domain.Event;

namespace Store.Shopping.Domain.Payments.Events;

public record PaymentCompletedEvent(Guid PaymentId, PaymentStatus Status) : IEvent;