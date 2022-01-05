using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Application.Orders;
using Store.Shopping.Application.Orders.Commands.PlaceOrder;
using Store.Shopping.Domain.Orders.Events;
using Store.Shopping.Domain.ValueObjects;
using Store.Shopping.Infrastructure.Entity;
using Xunit;
using Order = Store.Shopping.Domain.Orders.Order;

namespace Store.Shopping.Tests.Integration;

[Collection(nameof(StoreShoppingCombinedFixtureCollection))]
public class OrderCommandTests
{
    private readonly StoreShoppingCombinedFixture _fixture;

    public OrderCommandTests(StoreShoppingCombinedFixture fixture)
        => _fixture = Ensure.NotNull(fixture);

    [Fact]
    public async Task OrderPlaceCommand_Should_ReturnError_When_BuyerDoesNotExist()
    {
        IMediator mediator = _fixture.GetService<IMediator>();

        string customerNumber = Guid.NewGuid().ToString();
        string sessionId      = Guid.NewGuid().ToString();
        PaymentNumber paymentNumber = new(Guid.NewGuid());
        ShippingInfo shippingInformation = new(
            0,
            "full-name",
            "street-address",
            "city",
            "state-province",
            "postcode",
            "phone-number");


        #region Act
        
        OrderPlaceCommand orderPlaceCommand = new(customerNumber, sessionId, paymentNumber.Value, shippingInformation);
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
        PaymentNumber paymentNumber = new(Guid.NewGuid());
        ShippingInfo shippingInformation = new(
            0,
            "full-name",
            "street-address",
            "city",
            "state-province",
            "postcode",
            "phone-number");

        string[] productNumbers =
        {
            Guid.NewGuid().ToString(), 
            Guid.NewGuid().ToString(), 
            Guid.NewGuid().ToString()
        };
        
        #region Preconditions

        await _fixture.ProductsExist(
            new ProductEntity { CatalogueNumber = productNumbers[0], Available = true, Name = "Product0" },
            new ProductEntity { CatalogueNumber = productNumbers[1], Available = true, Name = "Product1" },
            new ProductEntity { CatalogueNumber = productNumbers[2], Available = true, Name = "Product2" });
        
        await _fixture.BuyerCreated(customerNumber, sessionId, productNumbers);
        
        #endregion
        
        #region Act

        OrderPlaceCommand orderPlaceCommand = new(customerNumber, sessionId, paymentNumber.Value, shippingInformation);
        Result<OrderPlaceResponse> orderPlaceResult = await mediator.Send(orderPlaceCommand);
        
        #endregion
        
        #region Assert
        
        Assert.NotNull(orderPlaceResult);
        Assert.True(orderPlaceResult.IsOk);

        OrderPlaceResponse response = orderPlaceResult.UnwrapOrDefault();
        Assert.NotNull(response);
        Assert.NotEqual(default, response.OrderId);

        var events = await _fixture.EventStoreFixture.Events($"{typeof(Order).FullName}-{response.OrderId}");
        Assert.NotEmpty(events);
        Assert.Contains(events, e => e is OrderPlacedEvent);

        var orderCreatedEvent = events.SingleOrDefault(e => e is OrderPlacedEvent) as OrderPlacedEvent;
        Assert.NotNull(orderCreatedEvent);
        Assert.Equal(response.OrderId, orderCreatedEvent.OrderId);
        Assert.Equal(customerNumber, orderCreatedEvent.CustomerNumber);
        Assert.Equal(paymentNumber.Value, orderCreatedEvent.PaymentId);
        
        var orderLinesCatalogueNumbers = orderCreatedEvent.OrderLines.Select(ol => ol.CatalogueNumber).ToArray();
        Assert.All(productNumbers, pn => orderLinesCatalogueNumbers.Contains(pn));

        Domain.Orders.ValueObjects.ShippingInfo shippingInfo = orderCreatedEvent.ShippingInfo;
        Assert.NotNull(shippingInfo);
        Assert.Equal(shippingInformation.CountryCode, shippingInfo.CountryCode);
        Assert.Equal(shippingInformation.FullName, shippingInfo.FullName);
        Assert.Equal(shippingInformation.StreetAddress, shippingInfo.StreetAddress);
        Assert.Equal(shippingInformation.City, shippingInfo.City);
        Assert.Equal(shippingInformation.StateProvince, shippingInfo.StateProvince);
        Assert.Equal(shippingInformation.Postcode, shippingInfo.Postcode);
        Assert.Equal(shippingInformation.PhoneNumber, shippingInfo.PhoneNumber);

        #endregion
    }
}