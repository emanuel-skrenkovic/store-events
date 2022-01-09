using System;
using Store.Core.Domain.ErrorHandling;
using Store.Inventory.Domain;
using Store.Inventory.Domain.ValueObjects;
using Xunit;

namespace Store.Inventory.Tests.Unit;

public class InventoryProductHelp
{
    [Fact]
    public void InventoryProduct_Should_CreateSuccessfully_With_ValidParameters()
    {
        ProductNumber productNumber = new(Guid.NewGuid());
        ProductInventory productInventory = ProductInventory.Create(productNumber);
        
        Assert.NotNull(productInventory);
        Assert.Equal(productNumber.Value, productInventory.Id);
        Assert.Equal(0, productInventory.Count);
    }

    [Fact]
    public void InventoryProduct_Should_AddToStock()
    {
        ProductNumber productNumber = new(Guid.NewGuid());
        ProductInventory productInventory = ProductInventory.Create(productNumber);

        const int count = 15;
        
        productInventory.AddToStock(count);
        
        Assert.NotNull(productInventory);
        Assert.Equal(count, productInventory.Count);
    }
    
    [Fact]
    public void InventoryProduct_Should_RemoveFromStock()
    {
        ProductNumber productNumber = new(Guid.NewGuid());
        ProductInventory productInventory = ProductInventory.Create(productNumber);

        const int count = 15;
        const int countToRemove = 12;
        
        productInventory.AddToStock(count);

        Result result = productInventory.RemoveFromStock(countToRemove);
        Assert.True(result.IsOk);
        
        Assert.NotNull(productInventory);
        Assert.Equal(count - countToRemove, productInventory.Count);
    }
    
    [Fact]
    public void InventoryProduct_Should_ReturnError_When_StockLessThanRemoveCount()
    {
        ProductNumber productNumber = new(Guid.NewGuid());
        ProductInventory productInventory = ProductInventory.Create(productNumber);

        const int count = 11;
        const int countToRemove = 12;
        
        productInventory.AddToStock(count);
        
        Result result = productInventory.RemoveFromStock(countToRemove);
        Assert.True(result.IsError);
        
        Assert.NotNull(productInventory);
        Assert.Equal(count, productInventory.Count);
    }
}