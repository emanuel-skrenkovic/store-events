using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Core.Domain.Event;
using Store.Shopping.Application.Buyers.Commands.AddItemToCart;
using Store.Shopping.Application.Buyers.Commands.RemoveItemFromCart;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.Events;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Store.Shopping.Infrastructure.Entity;
using Xunit;

namespace Store.Shopping.Tests.Integration;

[Collection(nameof(StoreShoppingCombinedFixtureCollection))]
public class BuyerCommandTests
{
    private readonly StoreShoppingCombinedFixture _fixture;

    public BuyerCommandTests(StoreShoppingCombinedFixture fixture)
        => _fixture = Ensure.NotNull(fixture);

    [Fact]
    public async Task BuyerAddItemToCartCommand_Should_CreateBuyer_And_AddItemToCart()
    {
        IMediator mediator = _fixture.GetService<IMediator>();

        string customerNumber         = Guid.NewGuid().ToString();
        string sessionId              = Guid.NewGuid().ToString();
        string productCatalogueNumber = Guid.NewGuid().ToString();
        
        #region Preconditions

        await _fixture.ProductsExist(new ProductEntity
        {
            CatalogueNumber = productCatalogueNumber, 
            Name = "Product", 
            Available = true
        });
        
        #endregion
        
        #region Act
        
        BuyerAddItemToCartCommand validRequest = new(
            customerNumber, 
            sessionId, 
            productCatalogueNumber);
        Result result = await mediator.Send(validRequest);
        
        #endregion
        
        #region Assert
        
        Assert.NotNull(result);
        Assert.True(result.IsOk);

        BuyerIdentifier buyerId = new(customerNumber, sessionId);
        
        // TODO: need better way for generating Id. Move Id to AggregateEntity class?
        List<IEvent> events = await _fixture.EventStoreFixture.Events($"{typeof(Buyer).FullName}-{buyerId}"); 
        
        Assert.Contains(events, e => e is BuyerCreatedEvent);
        Assert.Contains(events, e => e is BuyerCartItemAddedEvent);

        BuyerCartItemAddedEvent cartItemAddedEvent = events.First(e => e is BuyerCartItemAddedEvent) as BuyerCartItemAddedEvent;
        Assert.NotNull(cartItemAddedEvent);
        Assert.Equal(buyerId.ToString(), cartItemAddedEvent.BuyerId);
        Assert.Equal(productCatalogueNumber, cartItemAddedEvent.ProductCatalogueNumber);
        
        #endregion
    }
    
    [Fact]
    public async Task BuyerAddItemToCartCommand_Should_ReturnError_When_ProductNotAvailable()
    {
        IMediator mediator = _fixture.GetService<IMediator>();

        string customerNumber         = Guid.NewGuid().ToString();
        string sessionId              = Guid.NewGuid().ToString();
        string productCatalogueNumber = Guid.NewGuid().ToString();
        
        #region Preconditions

        await _fixture.ProductsExist(new ProductEntity
        {
            CatalogueNumber = productCatalogueNumber, 
            Name = "Product", 
            Available = false
        });
        
        #endregion
        
        #region Act
        
        BuyerAddItemToCartCommand validRequest = new(
            customerNumber, 
            sessionId, 
            productCatalogueNumber);
        Result result = await mediator.Send(validRequest);
        
        #endregion
        
        #region Assert
        
        Assert.NotNull(result);
        Assert.True(result.IsError);

        result.Match(null, Assert.IsType<Error>);

        BuyerIdentifier buyerId = new(customerNumber, sessionId);
        
        // TODO: need better way for generating Id. Move Id to AggregateEntity class?
        List<IEvent> events = await _fixture.EventStoreFixture.Events($"{typeof(Buyer).FullName}-{buyerId}"); 
        Assert.Null(events);
        
        #endregion
    }

    [Fact]
    public async Task BuyerRemoveItemFromCartCommand_Should_RemoveItemFromCart_When_ItemInCart()
    {
        IMediator mediator = _fixture.GetService<IMediator>();

        string customerNumber         = Guid.NewGuid().ToString();
        string sessionId              = Guid.NewGuid().ToString();
        string productCatalogueNumber = Guid.NewGuid().ToString();
        
        #region Preconditions
        
        await _fixture.ProductsExist(new ProductEntity
        {
            CatalogueNumber = productCatalogueNumber, 
            Name = "Product", 
            Available = true
        });
        
        BuyerAddItemToCartCommand addItemToCartCommand = new(
            customerNumber, 
            sessionId, 
            productCatalogueNumber);
        await mediator.Send(addItemToCartCommand);

        #endregion
        
        #region Act

        BuyerRemoveItemFromCartCommand removeItemFromCartCommand = new(
            customerNumber, 
            sessionId, 
            productCatalogueNumber);
        Result removeItemResult = await mediator.Send(removeItemFromCartCommand);
        
        #endregion
        
        #region Assert
            
        Assert.NotNull(removeItemResult);
        Assert.True(removeItemResult.IsOk);
        
        BuyerIdentifier buyerId = new(customerNumber, sessionId);
        
        List<IEvent> events = await _fixture.EventStoreFixture.Events($"{typeof(Buyer).FullName}-{buyerId}"); 
        
        Assert.Contains(events, e => e is BuyerCartItemRemovedEvent);

        BuyerCartItemRemovedEvent cartItemRemovedEvent = events.First(e => e is BuyerCartItemRemovedEvent) as BuyerCartItemRemovedEvent;
        Assert.NotNull(cartItemRemovedEvent);
        Assert.Equal(buyerId.ToString(), cartItemRemovedEvent.BuyerId);
        Assert.Equal(productCatalogueNumber, cartItemRemovedEvent.ProductCatalogueNumber);
        
        #endregion
    }
    
    [Fact]
    public async Task BuyerRemoveItemFromCartCommand_Should_ReturnError_When_BuyerNotFound()
    {
        IMediator mediator = _fixture.GetService<IMediator>();

        string customerNumber         = Guid.NewGuid().ToString();
        string sessionId              = Guid.NewGuid().ToString();
        string productCatalogueNumber = Guid.NewGuid().ToString();

        #region Act
    
        BuyerRemoveItemFromCartCommand removeItemFromCartCommand = new(
            customerNumber, 
            sessionId, 
            productCatalogueNumber);
        Result removeItemResult = await mediator.Send(removeItemFromCartCommand);
        
        #endregion
            
        #region Assert
        
        Assert.NotNull(removeItemResult);
        Assert.True(removeItemResult.IsError);

        removeItemResult.Match(null, Assert.IsType<NotFoundError>);
        
        #endregion
    }
}