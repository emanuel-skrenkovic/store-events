using System;
using Store.Order.Domain.Buyers;
using Store.Order.Domain.Buyers.Events;
using Xunit;

namespace Store.Order.Domain.Tests
{
    public class BuyerTests
    {
        [Fact]
        public void Buyer_Should_BeCreatedSuccessfully()
        {
            string buyerId = Guid.NewGuid().ToString();
            
            Buyer buyer = Buyer.Create(buyerId);
            
            Assert.NotNull(buyer);
            Assert.Equal(buyerId, buyer.Id);
            Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCreatedEvent));
        }

        [Fact]
        public void BuyerCartItem_Should_BeAddedSuccessfully()
        {
            Buyer buyer = Buyer.Create(Guid.NewGuid().ToString());

            string itemCatalogueNumber = Guid.NewGuid().ToString();
            Item item = new Item(new CatalogueNumber(itemCatalogueNumber));
            buyer.AddCartItem(item);
            
            Assert.True(buyer.Cart.Items.ContainsKey(item));
            Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCartItemAddedEvent));
        }
        
        [Fact]
        public void BuyerCartItem_Should_BeAddedSuccessfully_MultipleTimes()
        {
            Buyer buyer = Buyer.Create(Guid.NewGuid().ToString());

            string itemCatalogueNumber = Guid.NewGuid().ToString();
            Item item = new Item(new CatalogueNumber(itemCatalogueNumber));
            buyer.AddCartItem(item);
            
            Assert.True(buyer.Cart.Items.ContainsKey(item));
            Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCartItemAddedEvent));
            
            buyer.AddCartItem(item);
            Assert.True(buyer.Cart.Items.ContainsKey(item));
            Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCartItemAddedEvent));
            Assert.Equal(2u, buyer.Cart.Items[item]);
        }

        [Fact]
        public void BuyerCartItem_Should_Be_SuccessfullyRemoved()
        {
            Buyer buyer = Buyer.Create(Guid.NewGuid().ToString());

           string itemCatalogueNumber = Guid.NewGuid().ToString();
           Item item = new Item(new CatalogueNumber(itemCatalogueNumber));
           buyer.AddCartItem(item);

           buyer.RemoveCartItem(item);
           Assert.DoesNotContain(buyer.Cart.Items, c => c.Key == item);
        }
        
        [Fact]
        public void BuyerCartItemCount_Should_BeSuccessfullyCalculated_OnRemove()
        {
            Buyer buyer = Buyer.Create(Guid.NewGuid().ToString());

            string itemCatalogueNumber = Guid.NewGuid().ToString();
            Item item = new Item(new CatalogueNumber(itemCatalogueNumber));
            buyer.AddCartItem(item);
            
            Assert.True(buyer.Cart.Items.ContainsKey(item));
            Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCartItemAddedEvent));
            
            buyer.AddCartItem(item);
            Assert.True(buyer.Cart.Items.ContainsKey(item));
            Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCartItemAddedEvent));
            Assert.Equal(2u, buyer.Cart.Items[item]);
            
            buyer.RemoveCartItem(item);
            Assert.Contains(buyer.Cart.Items, c => c.Key == item);
            Assert.Equal(1u, buyer.Cart.Items[item]);
        }
    }
}