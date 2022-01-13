using System;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Orders.Events;
using Store.Shopping.Domain.Orders.ValueObjects;
using Store.Shopping.Domain.ValueObjects;
using Xunit;

namespace Store.Shopping.Tests.Unit;

public class OrderTests
{
    private Order CreateValidOrder()
    {
        CustomerNumber customerNumber = new(Guid.NewGuid().ToString());
        OrderNumber orderNumber = new(Guid.NewGuid());

        OrderLines orderLines = new(new []
        {
            new OrderLine(Guid.NewGuid().ToString(), 12, 2),
            new OrderLine(Guid.NewGuid().ToString(), 38, 4)
        });

        return Order.Create(orderNumber, customerNumber, orderLines);
    }
        
    [Fact]
    public void Order_Should_BeCreatedSuccessfully()
    {
        CustomerNumber customerNumber = new(Guid.NewGuid().ToString());
        OrderNumber orderNumber = new(Guid.NewGuid());
            
        OrderLines orderLines = new(new []
        {
            new OrderLine(Guid.NewGuid().ToString(), 12, 2),
            new OrderLine(Guid.NewGuid().ToString(), 38, 4)
        });
            
        Order order = Order.Create(orderNumber, customerNumber, orderLines);
        Assert.NotNull(order);
        Assert.Equal(orderNumber.Value, order.Id);
        Assert.Equal(customerNumber.Value, order.CustomerNumber);
        Assert.NotEmpty(order.OrderLines);
        
        Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderCreatedEvent));
    }

    [Fact]
    public void Order_Should_SubmitPayment()
    {
        Order order = CreateValidOrder();

        PaymentNumber paymentNumber = new(Guid.NewGuid());

        Result result = order.SubmitPayment(paymentNumber);
        Assert.NotNull(result);
        Assert.True(result.IsOk);
        
        Assert.Equal(paymentNumber.Value, order.PaymentId);
        Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderPaymentSubmittedEvent));
    }

    [Fact]
    public void Order_Should_SetShippingInformation()
    {
        Order order = CreateValidOrder();
        ShippingInformation shippingInformation = ShippingInformation.Create(
            0,
            "full-name",
            "street-address",
            "city",
            "state-province",
            "postcode",
            "phone-number").Unwrap(); 
        
        order.SetShippingInformation(shippingInformation);
        
        Assert.NotNull(order.ShippingInformation);
        Assert.Equal(shippingInformation.CountryCode, order.ShippingInformation.CountryCode);
        Assert.Equal(shippingInformation.FullName, order.ShippingInformation.FullName);
        Assert.Equal(shippingInformation.StreetAddress, order.ShippingInformation.StreetAddress);
        Assert.Equal(shippingInformation.City, order.ShippingInformation.City);
        Assert.Equal(shippingInformation.StateProvince, order.ShippingInformation.StateProvince);
        Assert.Equal(shippingInformation.Postcode, order.ShippingInformation.Postcode);
        Assert.Equal(shippingInformation.PhoneNumber, order.ShippingInformation.PhoneNumber);
        
        Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderShippingInformationSetEvent));
    }

    [Fact]
    public void Order_Should_ConfirmSuccessfully_When_PreconditionsMet()
    {
        Order order = CreateValidOrder();
        
        ShippingInformation shippingInformation = ShippingInformation.Create(
            0,
            "full-name",
            "street-address",
            "city",
            "state-province",
            "postcode",
            "phone-number").Unwrap(); 
        PaymentNumber paymentNumber = new(Guid.NewGuid());
        
        order.SetShippingInformation(shippingInformation);
        order.SubmitPayment(paymentNumber);

        Result result = order.Confirm();
        
        Assert.NotNull(result);
        Assert.True(result.IsOk);
        
        Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderConfirmedEvent));
    }

    [Fact]
    public void Order_ShouldNot_Confirm_When_NoPaymentSubmitted()
    {
        Order order = CreateValidOrder();
        
        ShippingInformation shippingInformation = ShippingInformation.Create(
            0,
            "full-name",
            "street-address",
            "city",
            "state-province",
            "postcode",
            "phone-number").Unwrap(); 
        order.SetShippingInformation(shippingInformation);

        Result result = order.Confirm();
        
        Assert.NotNull(result);
        Assert.True(result.IsError);
        
        Assert.DoesNotContain(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderConfirmedEvent));
    }
    
    [Fact]
    public void Order_ShouldNot_Confirm_When_ShippingInformationNotSet()
    {
        Order order = CreateValidOrder();
        
        PaymentNumber paymentNumber = new(Guid.NewGuid());
        order.SubmitPayment(paymentNumber);

        Result result = order.Confirm();
        
        Assert.NotNull(result);
        Assert.True(result.IsError);
        
        Assert.DoesNotContain(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderConfirmedEvent));
    }
}