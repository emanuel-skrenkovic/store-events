using Store.Core.Domain.Event;

namespace Store.Payments.Domain.Payments.Events;

public record PaymentVerifiedEvent(Guid PaymentId, PaymentStatus Status) : IEvent;