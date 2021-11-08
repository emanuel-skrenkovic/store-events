using System;
using Store.Core.Domain.Functional;
using Store.Core.Domain.Result;
using Store.Order.Domain.Buyers;
using Xunit;

namespace Store.Order.Domain.Tests
{
    public class CustomerOrderServiceTests
    {
        [Fact]
        public void CustomerOrderService_Should_ThrowOnNullBuyer()
        {
            ICustomerOrderService service = new CustomerOrderService();

            Assert.Throws<ArgumentNullException>(() => service.PlaceOrder(null));
        }

        [Fact]
        public void CustomerOrderService_Should_CreateValidOrder_For_ValidBuyer()
        {
            ICustomerOrderService service = new CustomerOrderService();
            
            CustomerNumber customerNumber = new CustomerNumber(Guid.NewGuid().ToString());
            
            Buyer buyer = Buyer.Create(Guid.NewGuid(), customerNumber);
            CatalogueNumber itemCatalogueNumber = new CatalogueNumber(Guid.NewGuid().ToString());
            Item item = new Item(itemCatalogueNumber);
            
            buyer.AddCartItem(item);

            Result<Orders.Order> placeOrderResult = service.PlaceOrder(buyer);
            
            Assert.False(placeOrderResult.IsError);

            placeOrderResult.Match<Unit>(order =>
            {
                Assert.NotNull(order);
                Assert.Equal(buyer.CustomerNumber, order.CustomerNumber);
                Assert.Contains(order.OrderLines, ol => ol.Key == itemCatalogueNumber);
                
                return Unit.Value;
            }, _ => Unit.Value);
        }

        [Fact]
        public void CustomerOrderService_Should_ReturnError_On_NoBuyerItems()
        {
            ICustomerOrderService service = new CustomerOrderService();
            
            CustomerNumber customerNumber = new CustomerNumber(Guid.NewGuid().ToString());
            
            Buyer buyer = Buyer.Create(Guid.NewGuid(), customerNumber);

            Result<Orders.Order> placeOrderResult = service.PlaceOrder(buyer);
            
            Assert.True(placeOrderResult.IsError);
            placeOrderResult.Match<Unit>(order => Unit.Value,
            error =>
            {
                Assert.NotNull(error);
                return Unit.Value;
            });
        }
    }
}