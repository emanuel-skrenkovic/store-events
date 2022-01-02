using System;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Orders.Events;
using Store.Shopping.Domain.Orders.ValueObjects;
using Store.Shopping.Domain.Payments.ValueObjects;
using Store.Shopping.Domain.ValueObjects;
using Xunit;

namespace Store.Shopping.Tests.Unit;

public class OrderTests
{
    /*
    private Order CreateValidOrder()
    {
        CustomerNumber customerNumber = new(Guid.NewGuid().ToString());
        OrderNumber orderNumber = new(Guid.NewGuid());
        PaymentNumber paymentNumber = new(Guid.NewGuid());
        ShippingInformation shippingInformation = ShippingInformation.Create(
            0,
            "full-name",
            "street-address",
            "city",
            "state-province",
            "postcode",
            "phone-number").Unwrap();

        OrderLines orderLines = new(new []
        {
            new OrderLine(Guid.NewGuid().ToString(), 12, 2),
            new OrderLine(Guid.NewGuid().ToString(), 38, 4)
        });

        return Order.Create(orderNumber, customerNumber, paymentNumber, orderLines, shippingInformation);
    }
    */
        
    [Fact]
    public void Order_Should_BeCreatedSuccessfully()
    {
        CustomerNumber customerNumber = new(Guid.NewGuid().ToString());
        OrderNumber orderNumber = new(Guid.NewGuid());
        PaymentNumber paymentNumber = new(Guid.NewGuid());
        ShippingInformation shippingInformation = ShippingInformation.Create(
            0,
            "full-name",
            "street-address",
            "city",
            "state-province",
            "postcode",
            "phone-number").Unwrap();

            
        OrderLines orderLines = new(new []
        {
            new OrderLine(Guid.NewGuid().ToString(), 12, 2),
            new OrderLine(Guid.NewGuid().ToString(), 38, 4)
        });
            
        Order order = Order.Create(orderNumber, customerNumber, paymentNumber, orderLines, shippingInformation);
        Assert.NotNull(order);
        Assert.Equal(orderNumber.Value, order.Id);
        Assert.Equal(customerNumber.Value, order.CustomerNumber);
        Assert.Equal(paymentNumber.Value, order.PaymentId);
        Assert.NotEmpty(order.OrderLines);
        
        Assert.NotNull(order.ShippingInformation);
        Assert.Equal(shippingInformation.CountryCode, order.ShippingInformation.CountryCode);
        Assert.Equal(shippingInformation.FullName, order.ShippingInformation.FullName);
        Assert.Equal(shippingInformation.StreetAddress, order.ShippingInformation.StreetAddress);
        Assert.Equal(shippingInformation.City, order.ShippingInformation.City);
        Assert.Equal(shippingInformation.StateProvince, order.ShippingInformation.StateProvince);
        Assert.Equal(shippingInformation.Postcode, order.ShippingInformation.Postcode);
        Assert.Equal(shippingInformation.PhoneNumber, order.ShippingInformation.PhoneNumber);
            
        Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderPlacedEvent));
    }
}