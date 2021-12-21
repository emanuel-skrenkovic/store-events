using System;
using Store.Order.Domain.Orders.ValueObjects;
using Store.Order.Domain.Payment;
using Store.Order.Domain.Payment.Events;
using Store.Order.Domain.Payment.ValueObjects;
using Store.Order.Domain.ValueObjects;
using Xunit;

namespace Store.Order.Domain.Tests;

public class PaymentTests
{
    private Payment.Payment CreateValidPayment()
    {
        CustomerNumber customerNumber = new(Guid.NewGuid().ToString());
        OrderNumber orderId = new(Guid.NewGuid());
        PaymentNumber paymentNumber = new(Guid.NewGuid());

        decimal amount = 15;

        return Payment.Payment.Create(
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

        Payment.Payment payment = Payment.Payment.Create(
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
        Payment.Payment payment = CreateValidPayment();

        payment.Complete();
        Assert.Contains(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCompletedEvent));
    }
        
    [Fact]
    public void Payment_ShouldCancel_WhenApproved()
    {
        Payment.Payment payment = CreateValidPayment();

        payment.Cancel();
        Assert.Equal(PaymentStatus.Cancelled, payment.Status);
        Assert.Contains(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCanceledEvent));
    }
        
    [Fact]
    public void Payment_ShouldNotComplete_WhenCanceled()
    {
        Payment.Payment payment = CreateValidPayment();

        payment.Cancel();
        payment.Complete();
            
        Assert.Equal(PaymentStatus.Cancelled, payment.Status);
        Assert.DoesNotContain(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCompletedEvent));
    }
        
    [Fact]
    public void Payment_ShouldNotCancel_WhenComplete()
    {
        Payment.Payment payment = CreateValidPayment();

        payment.Complete();
        payment.Cancel();
            
        Assert.Equal(PaymentStatus.Completed, payment.Status);
        Assert.DoesNotContain(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCanceledEvent));
    }
}