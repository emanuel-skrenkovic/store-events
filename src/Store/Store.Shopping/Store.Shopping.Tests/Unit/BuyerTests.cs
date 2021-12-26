using System;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.Events;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Xunit;

namespace Store.Shopping.Tests.Unit;

public class BuyerTests
{
    private Buyer CreateValidBuyer()
    {
        string customerNumber = Guid.NewGuid().ToString();
        string sessionId = Guid.NewGuid().ToString();
        BuyerIdentifier buyerId = new(customerNumber, sessionId);
        return Buyer.Create(buyerId);
    }
        
    [Fact]
    public void Buyer_Should_BeCreatedSuccessfully()
    {
        string customerNumber = Guid.NewGuid().ToString();
        string sessionId = Guid.NewGuid().ToString();
        BuyerIdentifier buyerId = new(customerNumber, sessionId);

        Buyer buyer = Buyer.Create(buyerId);
            
        Assert.NotNull(buyer);
        Assert.Equal(customerNumber, buyer.CustomerNumber);
        Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCreatedEvent));
    }

    [Fact]
    public void BuyerCartItem_Should_BeAddedSuccessfully()
    {
        Buyer buyer = CreateValidBuyer();

        string itemCatalogueNumber = Guid.NewGuid().ToString();
        buyer.AddCartItem(new CatalogueNumber(itemCatalogueNumber));
            
        Assert.True(buyer.CartItems.ContainsKey(itemCatalogueNumber));
        Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCartItemAddedEvent));
    }
        
    [Fact]
    public void BuyerCartItem_Should_BeAddedSuccessfully_MultipleTimes()
    {
        Buyer buyer = CreateValidBuyer();

        string itemCatalogueNumber = Guid.NewGuid().ToString();
        buyer.AddCartItem(new CatalogueNumber(itemCatalogueNumber));
            
        Assert.True(buyer.CartItems.ContainsKey(itemCatalogueNumber));
        Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCartItemAddedEvent));
            
        buyer.AddCartItem(new CatalogueNumber(itemCatalogueNumber));
        Assert.True(buyer.CartItems.ContainsKey(itemCatalogueNumber));
        Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCartItemAddedEvent));
        Assert.Equal(2u, buyer.CartItems[itemCatalogueNumber]);
    }

    [Fact]
    public void BuyerCartItem_Should_Be_SuccessfullyRemoved()
    {
        Buyer buyer = CreateValidBuyer();

        string itemCatalogueNumber = Guid.NewGuid().ToString();
        buyer.AddCartItem(new CatalogueNumber(itemCatalogueNumber));

        buyer.RemoveCartItem(new CatalogueNumber(itemCatalogueNumber));
        Assert.DoesNotContain(buyer.CartItems, c => c.Key == itemCatalogueNumber);
    }
        
    [Fact]
    public void BuyerCartItemCount_Should_BeSuccessfullyCalculated_OnRemove()
    {
        Buyer buyer = CreateValidBuyer();

        string itemCatalogueNumber = Guid.NewGuid().ToString();
        buyer.AddCartItem(new CatalogueNumber(itemCatalogueNumber));
            
        Assert.True(buyer.CartItems.ContainsKey(itemCatalogueNumber));
        Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCartItemAddedEvent));
            
        buyer.AddCartItem(new CatalogueNumber(itemCatalogueNumber));
        Assert.True(buyer.CartItems.ContainsKey(itemCatalogueNumber));
        Assert.Contains(buyer.GetUncommittedEvents(), e => e.GetType() == typeof(BuyerCartItemAddedEvent));
        Assert.Equal(2u, buyer.CartItems[itemCatalogueNumber]);
            
        buyer.RemoveCartItem(new CatalogueNumber(itemCatalogueNumber));
        Assert.Contains(buyer.CartItems, c => c.Key == itemCatalogueNumber);
        Assert.Equal(1u, buyer.CartItems[itemCatalogueNumber]);
    }
}