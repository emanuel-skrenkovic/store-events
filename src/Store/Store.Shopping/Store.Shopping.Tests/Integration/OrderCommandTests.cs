using System;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Application.Buyers.Commands.AddItemToCart;
using Store.Shopping.Application.Orders.Commands.PlaceOrder;
using Xunit;

namespace Store.Shopping.Tests.Integration;

[Collection(StoreShoppingCollection.Name)]
public class OrderCommandTests : IDisposable
{
    private readonly StoreShoppingFixture _fixture;

    public OrderCommandTests(StoreShoppingFixture fixture)
        => _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

    [Fact]
    public async Task OrderPlaceCommand_Should_ReturnError_When_BuyerDoesNotExist()
    {
        IMediator mediator = _fixture.GetService<IMediator>();

        const string customerNumber = "1234";
        const string sessionId      = "4321";

        #region Act
        
        OrderPlaceCommand orderPlaceCommand = new(customerNumber, sessionId);
        Result orderPlaceResult = await mediator.Send(orderPlaceCommand);
        
        #endregion
        
        #region Assert
        
        Assert.NotNull(orderPlaceResult);
        Assert.True(orderPlaceResult.IsError);

        orderPlaceResult.Match(null, Assert.IsType<NotFoundError>);
        
        #endregion
    }

    /*
    [Fact]
    public async Task OrderPlaceCommand_Should_CreateOrder_When_PreconditionsMet()
    {
        var mediator = _fixture.GetService<IMediator>();
        
        const string customerNumber = "1234";
        const string sessionId      = "4321";

        string[] productNumbers = { "1234", "4321", "asdf" };
        
        #region Preconditions

        await BuyerCreated(customerNumber, sessionId, productNumbers);
        
        #endregion
        
        #region Act

        OrderPlaceCommand orderPlaceCommand = new(customerNumber, sessionId);
        Result orderPlaceResult = await mediator.Send(orderPlaceCommand);
        
        #endregion
        
        #region Assert
        
        Assert.NotNull(orderPlaceResult);
        Assert.True(orderPlaceResult.IsOk);
        
        #endregion
    }

    private async Task BuyerCreated(string customerNumber, string sessionId, params string[] productCatalogueNumbers)
    {
        var mediator = _fixture.GetService<IMediator>();

        foreach (string catalogueNumber in productCatalogueNumbers)
        {
            BuyerAddItemToCartCommand validRequest = new(
                customerNumber, 
                sessionId, 
                catalogueNumber);
            await mediator.Send(validRequest); 
        }
    }
    */

    public void Dispose()
    {
        _fixture.EventStoreFixture.CleanAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}