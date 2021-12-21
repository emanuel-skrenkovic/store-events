using System;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Domain.Orders.ValueObjects;
using Store.Order.Domain.Payment.Events;
using Store.Order.Domain.Payment.ValueObjects;
using Store.Order.Domain.ValueObjects;

namespace Store.Order.Domain.Payment;

public class Payment : AggregateEntity<Guid>
{
    public Guid OrderId { get; private set; }
        
    public string CustomerNumber { get; private set; }
        
    public string BuyerEmailAddress { get; private set; }
        
    public string BillingAddress { get; private set; }
        
    public PaymentStatus Status { get; private set; }
        
    public decimal Amount { get; private set; }

    public static Payment Create(
        PaymentNumber paymentNumber, 
        CustomerNumber customerNumber, 
        OrderNumber orderNumber,
        decimal amount)
    {
        Payment payment = new();
        payment.ApplyEvent(new PaymentCreatedEvent(
            paymentNumber.Value, 
            customerNumber.Value, 
            orderNumber.Value, 
            amount,
            PaymentStatus.Approved));

        return payment;
    }

    private void Apply(PaymentCreatedEvent domainEvent)
    {
        Id = domainEvent.PaymentId;
        CustomerNumber = domainEvent.CustomerNumber;
        OrderId = domainEvent.OrderId;
        Amount = domainEvent.Amount;
        Status = domainEvent.Status;
    }

    public Result Complete()
    {
        if (Status != PaymentStatus.Approved) return new Error("Cannot complete unapproved payment.");
        ApplyEvent(new PaymentCompletedEvent(Id, PaymentStatus.Completed));
            
        return Result.Ok();
    }

    private void Apply(PaymentCompletedEvent domainEvent)
        => Status = domainEvent.Status;

    public Result Cancel()
    {
        if (Status == PaymentStatus.Completed) return new Error("Payment was already completed.");
            
        ApplyEvent(new PaymentCanceledEvent(Id, PaymentStatus.Cancelled));
        return Result.Ok();
    }

    private void Apply(PaymentCanceledEvent domainEvent)
        => Status = domainEvent.Status;
        
    #region Wiring
        
    protected override void RegisterAppliers()
    {
        RegisterApplier<PaymentCreatedEvent>(Apply);
        RegisterApplier<PaymentCompletedEvent>(Apply);
        RegisterApplier<PaymentCanceledEvent>(Apply);
    }
        
    #endregion
}