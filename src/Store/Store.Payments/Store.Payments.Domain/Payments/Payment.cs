using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Payments.Domain.Payments.Events;
using Store.Payments.Domain.Payments.ValueObjects;

namespace Store.Payments.Domain.Payments;

public class Payment : AggregateEntity<Guid>
{
    public Guid OrderId { get; private set; }
    
    public Refund RefundInfo { get; private set; }

    public PaymentStatus Status { get; private set; }
    
    public string Source { get; private set; }
    
    public decimal Amount { get; private set; }
    
    public string Note { get; private set; }
    
    public static Payment Create(
        PaymentNumber paymentNumber,
        OrderId orderId,
        Source source,
        Amount amount,
        string note = null)
    {
        Payment payment = new();
        payment.ApplyEvent(new PaymentCreatedEvent(
            paymentNumber.Value,
            orderId.Value,
            source.Value,
            amount.Value,
            PaymentStatus.Created,
            note));

        return payment;
    }

    private void Apply(PaymentCreatedEvent domainEvent)
    {
        Id = domainEvent.PaymentId;
        OrderId = domainEvent.OrderId;
        Source = domainEvent.Source;
        Amount = domainEvent.Amount;
        Status = domainEvent.Status;
        Note = domainEvent.Note;
    }

    // TODO: this seems stupid.
    public Result<Refund> Refund(string note = null)
    {
        if (Status == PaymentStatus.Refunded) return new Error($"Payment is already refunded. See refund id: '{RefundInfo.Id}';");
        
        Refund refund = new()
        {
            Id = Guid.NewGuid(),
            PaymentId = Id,
            Amount = Amount,
            Note = note
        };
        ApplyEvent(new PaymentRefundedEvent(Id, refund, PaymentStatus.Refunded));

        return refund;
    }
    
    private void Apply(PaymentRefundedEvent domainEvent)
    {
        RefundInfo = domainEvent.Refund;
        Status = domainEvent.Status;
    }

    public Result Verify()
    {
        if (Status != PaymentStatus.Created) return new Error($"Payment in state '{Status}' cannot be verified.");
        ApplyEvent(new PaymentVerifiedEvent(Id, PaymentStatus.Verified));
        
        return Result.Ok();
    }

    private void Apply(PaymentVerifiedEvent domainEvent) => Status = domainEvent.Status;
    
    protected override void RegisterAppliers()
    {
        RegisterApplier<PaymentCreatedEvent>(Apply);
        RegisterApplier<PaymentRefundedEvent>(Apply);
        RegisterApplier<PaymentVerifiedEvent>(Apply);
    }
}