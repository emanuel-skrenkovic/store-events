using System;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Application.Buyers.Commands.AddItemToCart;
using Store.Shopping.Application.Orders.Commands.PlaceOrder;
using Store.Shopping.Infrastructure.Entity;
using Xunit;

namespace Store.Shopping.Tests.Integration;

public class OrderCommandTests : IClassFixture<StoreShoppingCombinedFixture>
{
    private readonly StoreShoppingCombinedFixture _fixture;

    public OrderCommandTests(StoreShoppingCombinedFixture fixture)
        => _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

    [Fact]
    public async Task OrderPlaceCommand_Should_ReturnError_When_BuyerDoesNotExist()
    {
        IMediator mediator = _fixture.GetService<IMediator>();

        string customerNumber = Guid.NewGuid().ToString();
        string sessionId      = Guid.NewGuid().ToString();

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

    [Fact]
    public async Task OrderPlaceCommand_Should_CreateOrder_When_PreconditionsMet()
    {
        var mediator = _fixture.GetService<IMediator>();
        
        string customerNumber = Guid.NewGuid().ToString();
        string sessionId      = Guid.NewGuid().ToString();

        string[] productNumbers = { "1234", "4321", "asdf" };
        
        #region Preconditions

        await ProductsExist(
            new ProductEntity { CatalogueNumber = productNumbers[0], Available = true, Name = "Product0" },
            new ProductEntity { CatalogueNumber = productNumbers[1], Available = true, Name = "Product1" },
            new ProductEntity { CatalogueNumber = productNumbers[2], Available = true, Name = "Product2" });
        
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

    private async Task ProductsExist(params ProductEntity[] products)
    {
        var context = _fixture.PostgresFixture.Context;

        foreach (ProductEntity product in products)
            context.Add(product);

        await context.SaveChangesAsync();
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
}