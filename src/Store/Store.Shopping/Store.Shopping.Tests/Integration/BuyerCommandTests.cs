using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Core.Domain.Event;
using Store.Shopping.Application.Buyers.Commands.AddItemToCart;
using Store.Shopping.Application.Buyers.Commands.RemoveItemFromCart;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.Events;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Xunit;

namespace Store.Shopping.Tests.Integration;

[Collection(StoreShoppingCollection.Name)]
public class BuyerCommandTests : IAsyncLifetime
{
    private readonly StoreShoppingFixture _fixture;

    public BuyerCommandTests(StoreShoppingFixture fixture)
        => _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

    [Fact]
    public async Task BuyerAddItemToCartCommand_Should_CreateBuyer_And_AddItemToCart()
    {
        IMediator mediator = _fixture.GetService<IMediator>();

        const string customerNumber         = "1234";
        const string sessionId              = "4321";
        const string productCatalogueNumber = "asdf";
        
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
    public async Task BuyerRemoveItemFromCartCommand_Should_RemoveItemFromCart_When_ItemInCart()
    {
        IMediator mediator = _fixture.GetService<IMediator>();

        const string customerNumber         = "1234";
        const string sessionId              = "4321";
        const string productCatalogueNumber = "asdf";
        
        #region Preconditions
        
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

        const string customerNumber         = "1234";
        const string sessionId              = "4321";
        const string productCatalogueNumber = "asdf";

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

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _fixture.EventStoreFixture.CleanAsync();
    }
}