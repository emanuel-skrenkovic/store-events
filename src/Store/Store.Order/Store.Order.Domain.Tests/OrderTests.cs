using System;
using Store.Order.Domain.Orders;
using Store.Order.Domain.Orders.Events;
using Xunit;

namespace Store.Order.Domain.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_Should_BeCreatedSuccessfully()
        {
            CustomerNumber customerNumber = new CustomerNumber(Guid.NewGuid().ToString());
            Guid orderId = Guid.NewGuid();
            
            Orders.Order order = Orders.Order.Create(orderId, customerNumber);
            Assert.NotNull(order);
            Assert.Equal(orderId, order.Id);
            Assert.Equal(customerNumber, order.CustomerNumber);
            
            Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderCreatedEvent));
        }

        [Fact]
        public void Order_OrderLine_Should_BeAddedSuccessfully()
        {
            CustomerNumber customerNumber = new CustomerNumber(Guid.NewGuid().ToString());
            Guid orderId = Guid.NewGuid();
            
            Orders.Order order = Orders.Order.Create(orderId, customerNumber);

            CatalogueNumber catalogueNumber = new CatalogueNumber(Guid.NewGuid().ToString());
            Item item = new Item(catalogueNumber);
            OrderLine orderLine = new OrderLine(item, 1);
            
            order.AddOrderLine(orderLine);

            Assert.Contains(order.OrderLines, o => o.Key == catalogueNumber);
            Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderOrderLineAddedEvent));
        }
        
        [Fact]
        public void Order_ShippingInformation_Should_BeAddedSuccessfully()
        {
            CustomerNumber customerNumber = new CustomerNumber(Guid.NewGuid().ToString());
            Guid orderId = Guid.NewGuid();
            
            Orders.Order order = Orders.Order.Create(orderId, customerNumber);
            
            ShippingInformation shippingInformation = new ShippingInformation(
                1, "Test Full Name", "Street Address 1", "MadeUp City", "TotallyFakeProvince", "NonExistantPostcode", "CallMe");
            
            order.SetShippingInformation(shippingInformation);
            Assert.Equal(shippingInformation, order.ShippingInformation);
            Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderShippingInformationAddedEvent));
            Assert.DoesNotContain(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderShippingInformationChangedEvent));
        }
        
        [Fact]
        public void Order_ShippingInformation_Should_BeChangedSuccessfully()
        {
            CustomerNumber customerNumber = new CustomerNumber(Guid.NewGuid().ToString());
            Guid orderId = Guid.NewGuid();
            
            Orders.Order order = Orders.Order.Create(orderId, customerNumber);
            
            ShippingInformation shippingInformation = new ShippingInformation(
                1, "Test Full Name", "Street Address 1", "MadeUp City", "TotallyFakeProvince", "NonExistantPostcode", "CallMe");
            
            order.SetShippingInformation(shippingInformation);
            
            Assert.Equal(shippingInformation, order.ShippingInformation);
            Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderShippingInformationAddedEvent));
            Assert.DoesNotContain(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderShippingInformationChangedEvent));

            ShippingInformation changedShippingInformation = shippingInformation with { Postcode = "TotallyCorrectPostCodeNow" };
            order.SetShippingInformation(changedShippingInformation);
            
            Assert.Equal(changedShippingInformation, order.ShippingInformation);
            Assert.Contains(order.GetUncommittedEvents(), e => e.GetType() == typeof(OrderShippingInformationChangedEvent));
        }
    }
}