using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Inventory.Application.Commands;
using Store.Inventory.Domain;
using Store.Inventory.Domain.Events;
using Xunit;

namespace Store.Inventory.Tests.Integration;

public class InventoryEventStoreIntegrationTests : IClassFixture<InventoryEventStoreFixture>
{
    private readonly InventoryEventStoreFixture _fixture;

    public InventoryEventStoreIntegrationTests(InventoryEventStoreFixture fixture)
        => _fixture = Ensure.NotNull(fixture);
    
    [Fact]
    public async Task ProductInventoryAddToStockCommand_Should_CreateProductInventoryAndAddCount()
    {
        var mediator = _fixture.GetService<IMediator>();
        
        Guid productId  = Guid.NewGuid();
        const int count = 15;
        
        #region Act
        
        ProductInventoryAddToStockCommand command = new(productId, count);
        Result result = await mediator.Send(command);

        #endregion

        #region Assert
        
        Assert.NotNull(result);
        Assert.True(result.IsOk);

        var events = await _fixture.EventStoreFixture.Events($"{typeof(ProductInventory).FullName}-{productId}");
        Assert.NotEmpty(events);
        Assert.Contains(events, e => e is ProductInventoryCreatedEvent);
        Assert.Contains(events, e => e is ProductInventoryAddedToStockEvent);

        var createdEvent = events.SingleOrDefault(e => e is ProductInventoryCreatedEvent) as ProductInventoryCreatedEvent;
        Assert.NotNull(createdEvent);
        Assert.Equal(productId, createdEvent.ProductId);
        
        var addedToStockEvent = events.SingleOrDefault(e => e is ProductInventoryAddedToStockEvent) as ProductInventoryAddedToStockEvent;
        Assert.NotNull(addedToStockEvent);
        Assert.Equal(productId, addedToStockEvent.ProductId);
        Assert.Equal(count, addedToStockEvent.Count);

        #endregion
    }

    [Fact]
    public async Task ProductInventoryRemoveFromStockCommand_Should_ReturnErrorResult_When_ProductInventoryDoesNotExist()
    {
        var mediator = _fixture.GetService<IMediator>();
        
        Guid productId  = Guid.NewGuid();
        const int count = 15;
        
        #region Act
        
        ProductInventoryRemoveFromStockCommand command = new(productId, count);
        Result result = await mediator.Send(command);

        #endregion

        #region Assert
        
        Assert.NotNull(result);
        Assert.True(result.IsError);

        var events = await _fixture.EventStoreFixture.Events($"{typeof(ProductInventory).FullName}-{productId}");
        Assert.Null(events);

        #endregion 
    }
    
    [Fact]
    public async Task ProductInventoryRemoveFromStockCommand_Should_RemoveCount_When_ProductInventoryExists()
    {
        var mediator = _fixture.GetService<IMediator>();
        
        Guid productId  = Guid.NewGuid();
        const int initialCount = 15;
        const int countToRemove = 12;
        
        #region Prerequisites
        
        ProductInventoryAddToStockCommand addToStockCommand = new(productId, initialCount);
        await mediator.Send(addToStockCommand);
        
        #endregion
        
        #region Act
        
        ProductInventoryRemoveFromStockCommand command = new(productId, countToRemove);
        Result result = await mediator.Send(command);

        #endregion

        #region Assert
        
        Assert.NotNull(result);
        Assert.True(result.IsOk);

        var events = await _fixture.EventStoreFixture.Events($"{typeof(ProductInventory).FullName}-{productId}");
        Assert.NotEmpty(events);
        Assert.Contains(events, e => e is ProductInventoryRemovedFromStockEvent);
        
        var removedFromStockEvent = events.SingleOrDefault(e => e is ProductInventoryRemovedFromStockEvent) as ProductInventoryRemovedFromStockEvent;
        Assert.NotNull(removedFromStockEvent);
        Assert.Equal(productId, removedFromStockEvent.ProductId);
        Assert.Equal(countToRemove, removedFromStockEvent.Count);

        #endregion 
    }
}