using Store.Core.Domain.Event;

namespace Store.Payments.Domain.Payments.Events;

public record PaymentRefundedEvent(Guid PaymentId, Refund Refund, PaymentStatus Status) : IEvent;