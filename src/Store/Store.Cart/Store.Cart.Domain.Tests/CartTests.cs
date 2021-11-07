using System;
using Store.Cart.Domain.Events;
using Xunit;

namespace Store.Cart.Domain.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Create_Cart()
        {
            Guid id = Guid.NewGuid();
            Guid customerId = Guid.NewGuid();
            
            Cart cart = Cart.Create(id, customerId);
            Assert.NotNull(cart);
            
            Assert.Equal(customerId, cart.CustomerId);
            Assert.Equal(id, cart.Id);
            
            Assert.Contains(cart.GetUncommittedEvents(), e => e.GetType() == typeof(CartCreatedEvent));
        }
    }
}