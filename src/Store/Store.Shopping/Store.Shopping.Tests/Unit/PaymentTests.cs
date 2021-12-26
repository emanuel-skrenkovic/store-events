using System;
using Store.Shopping.Domain.Orders.ValueObjects;
using Store.Shopping.Domain.Payments;
using Store.Shopping.Domain.Payments.Events;
using Store.Shopping.Domain.Payments.ValueObjects;
using Store.Shopping.Domain.ValueObjects;
using Xunit;

namespace Store.Shopping.Tests.Unit;

public class PaymentTests
{
    private Payment CreateValidPayment()
    {
        CustomerNumber customerNumber = new(Guid.NewGuid().ToString());
        OrderNumber orderId = new(Guid.NewGuid());
        PaymentNumber paymentNumber = new(Guid.NewGuid());

        decimal amount = 15;

        return Payment.Create(
            paymentNumber,
            customerNumber, 
            orderId, 
            amount);
    }
        
    [Fact]
    public void Payment_Should_BeCreatedSuccessfully()
    {
        CustomerNumber customerNumber = new(Guid.NewGuid().ToString());
        OrderNumber orderId = new(Guid.NewGuid());
        PaymentNumber paymentNumber = new(Guid.NewGuid());

        decimal amount = 15;

        Payment payment = Payment.Create(
            paymentNumber,
            customerNumber, 
            orderId, 
            amount);
            
        Assert.NotNull(payment);
        Assert.Equal(customerNumber.Value, payment.CustomerNumber);
        Assert.Equal(orderId.Value, payment.OrderId);
        Assert.Equal(PaymentStatus.Approved, payment.Status);
        Assert.Contains(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCreatedEvent));
    }

    [Fact]
    public void Payment_ShouldComplete_WhenApproved()
    {
        Payment payment = CreateValidPayment();

        payment.Complete();
        Assert.Contains(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCompletedEvent));
    }
        
    [Fact]
    public void Payment_ShouldCancel_WhenApproved()
    {
        Payment payment = CreateValidPayment();

        payment.Cancel();
        Assert.Equal(PaymentStatus.Cancelled, payment.Status);
        Assert.Contains(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCanceledEvent));
    }
        
    [Fact]
    public void Payment_ShouldNotComplete_WhenCanceled()
    {
        Payment payment = CreateValidPayment();

        payment.Cancel();
        payment.Complete();
            
        Assert.Equal(PaymentStatus.Cancelled, payment.Status);
        Assert.DoesNotContain(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCompletedEvent));
    }
        
    [Fact]
    public void Payment_ShouldNotCancel_WhenComplete()
    {
        Payment payment = CreateValidPayment();

        payment.Complete();
        payment.Cancel();
            
        Assert.Equal(PaymentStatus.Completed, payment.Status);
        Assert.DoesNotContain(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCanceledEvent));
    }
}