using Store.Core.Domain.Event;

namespace Store.Payments.Domain.Payments.Events;

public record PaymentCompletedEvent(Guid PaymentId, PaymentStatus Status) : IEvent;