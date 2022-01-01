using Store.Core.Domain.Event;

namespace Store.Payments.Domain.Payments.Events;

public record PaymentCreatedEvent(
    Guid PaymentId, 
    string Source, 
    decimal Amount, 
    PaymentStatus Status,
    string Note = null) : IEvent;