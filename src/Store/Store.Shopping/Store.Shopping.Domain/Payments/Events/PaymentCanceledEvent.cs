using System;
using Store.Core.Domain.Event;

namespace Store.Shopping.Domain.Payments.Events;

public record PaymentCanceledEvent(Guid PaymentId, PaymentStatus Status) : IEvent;