using System;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Orders.Events;
using Store.Shopping.Domain.Orders.ValueObjects;
using Store.Shopping.Domain.ValueObjects;
using Xunit;

namespace Store.Shopping.Domain.Tests;

public class OrderTests
{
    private Domain.Orders.Order CreateValidOrder()
    {
        CustomerNumber customerNumber = new(Guid.NewGuid().ToString());
        OrderNumber orderNumber = new(Guid.NewGuid());

        OrderLines orderLines = new(new []
        {
            new OrderLine(Guid.NewGuid().ToString(), 12, 2),
            new OrderLine(Guid.NewGuid().ToString(), 38, 4)
        });

        return Domain.Orders.Order.Create(orderNumber, customerNumber, orderLines);
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

            
        Domain.Orders.Order order = Domain.Orders.Order.Create(orderNumber, customerNumber, orderLines);
        Assert.NotNull(order);
        Assert.Equal(orderNumber.Value, order.Id);
        Assert.Equal(customerNumber.Value, order.CustomerNumber);
        Assert.NotEmpty(order.OrderLines);
            
        Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderCreatedEvent));
    }
        
    [Fact]
    public void Order_ShippingInformation_Should_BeAddedSuccessfully()
    {
        Domain.Orders.Order order = CreateValidOrder();
            
        ShippingInformation shippingInformation = new ShippingInformation(
            1, "Test Full Name", "Street Address 1", "MadeUp City", "TotallyFakeProvince", "NonExistantPostcode", "CallMe");
            
        order.SetShippingInformation(shippingInformation);
        Assert.Equal(shippingInformation, order.ShippingInformation);
        Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderShippingInformationSetEvent));
    }
        
    [Fact]
    public void Order_ShippingInformation_Should_BeChangedSuccessfully()
    {
        Domain.Orders.Order order = CreateValidOrder();
            
        ShippingInformation shippingInformation = new ShippingInformation(
            1, "Test Full Name", "Street Address 1", "MadeUp City", "TotallyFakeProvince", "NonExistantPostcode", "CallMe");
            
        order.SetShippingInformation(shippingInformation);
            
        Assert.Equal(shippingInformation, order.ShippingInformation);
        Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderShippingInformationSetEvent));

        ShippingInformation changedShippingInformation = shippingInformation with { Postcode = "TotallyCorrectPostCodeNow" };
        order.SetShippingInformation(changedShippingInformation);
            
        Assert.Equal(changedShippingInformation, order.ShippingInformation);
        Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderShippingInformationSetEvent));
    }
}