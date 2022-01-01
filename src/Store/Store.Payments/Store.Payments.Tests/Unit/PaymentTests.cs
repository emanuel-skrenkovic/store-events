using System;
using System.Diagnostics;
using System.Linq;
using Store.Core.Domain.ErrorHandling;
using Store.Payments.Domain.Payments;
using Store.Payments.Domain.Payments.Events;
using Store.Payments.Domain.Payments.ValueObjects;
using Xunit;

namespace Store.Payments.Tests.Unit;

public class PaymentTests
{
    private Payment ValidPayment()
    {
        PaymentNumber paymentNumber = new(Guid.NewGuid());
        Source source = new("valid-source");
        Amount amount = new(15m);
        string note = "valid-note";
        
        return Payment.Create(
            paymentNumber,
            source,
            amount,
            note);
    }
    
    [Fact]
    public void Payment_Should_BeCreatedSuccessfully_With_ValidParameters()
    {
        PaymentNumber paymentNumber = new(Guid.NewGuid());
        Source source = new("valid-source");
        Amount amount = new(15m);
        string note = "valid-note";
        
        Payment payment = Payment.Create(
            paymentNumber,
            source,
            amount,
            note);
        
        Assert.NotNull(payment);
        
        Assert.Equal(paymentNumber.Value, payment.Id);
        Assert.Equal(source.Value, payment.Source);
        Assert.Equal(amount.Value, payment.Amount);
        Assert.Equal(note, payment.Note);
        
        Assert.NotEmpty(payment.GetUncommittedEvents());
        Assert.Contains(payment.GetUncommittedEvents(), e => e is PaymentCreatedEvent);
        
        var @event = payment.GetUncommittedEvents().SingleOrDefault(e => e is PaymentCreatedEvent) as PaymentCreatedEvent;
        Assert.NotNull(@event);
        
        Debug.Assert(@event != null);
        
        Assert.Equal(paymentNumber.Value, @event.PaymentId);
        Assert.Equal(source.Value, @event.Source);
        Assert.Equal(amount.Value, @event.Amount);
        Assert.Equal(note, @event.Note);
    }

    [Fact]
    public void Payment_Should_Complete_When_NotRefunded()
    {
        Payment payment = ValidPayment();

        Result completePaymentResult = payment.Complete();
        
        Assert.True(completePaymentResult.IsOk);
        Assert.Equal(PaymentStatus.Completed, payment.Status);
        
        Assert.NotEmpty(payment.GetUncommittedEvents());
        Assert.Contains(payment.GetUncommittedEvents(), e => e is PaymentCompletedEvent);
    }

    [Fact]
    public void Payment_Should_ReturnError_On_Complete_When_Refunded()
    {
        Payment payment = ValidPayment();

        payment.Refund();
        
        Result completePaymentResult = payment.Complete();
        
        Assert.True(completePaymentResult.IsError);
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
        
        Assert.NotEmpty(payment.GetUncommittedEvents());
        Assert.DoesNotContain(payment.GetUncommittedEvents(), e => e is PaymentCompletedEvent);
    }

    [Fact]
    public void Payment_Should_Refund_When_NotAlreadyRefunded()
    {
        Payment payment = ValidPayment();

        Result<Refund> refundPaymentResult = payment.Refund();
        
        Assert.True(refundPaymentResult.IsOk);
        Assert.NotNull(refundPaymentResult.UnwrapOrDefault());
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
        
        Assert.NotEmpty(payment.GetUncommittedEvents());
        Assert.Contains(payment.GetUncommittedEvents(), e => e is PaymentRefundedEvent); 
    }

    [Fact]
    public void Payment_Should_ReturnError_On_Refund_When_PaymentAlreadyRefunded()
    {
        Payment payment = ValidPayment();

        payment.Refund();
        Result<Refund> refundPaymentResult = payment.Refund();
        
        Assert.True(refundPaymentResult.IsError);
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
        
        Assert.NotEmpty(payment.GetUncommittedEvents());
        Assert.Single(payment.GetUncommittedEvents(), e => e is PaymentRefundedEvent); 
    }
}