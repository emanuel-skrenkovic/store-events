using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Application.Orders;
using Store.Shopping.Application.Orders.Commands.CreateOrder;
using Store.Shopping.Application.Orders.Commands.SetShippingInformation;
using Store.Shopping.Application.Orders.Commands.SubmitPayment;
using Store.Shopping.Domain.Orders.Events;
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
    public async Task OrderCreateCommand_Should_ReturnError_When_BuyerDoesNotExist()
    {
        IMediator mediator = _fixture.GetService<IMediator>();

        string customerNumber = Guid.NewGuid().ToString();
        string sessionId      = Guid.NewGuid().ToString();

        #region Act
        
        OrderCreateCommand orderCreateCommand = new(customerNumber, sessionId);
        Result orderCreateResult = await mediator.Send(orderCreateCommand);
        
        #endregion
        
        #region Assert
        
        Assert.NotNull(orderCreateResult);
        Assert.True(orderCreateResult.IsError);

        orderCreateResult.Match(null, Assert.IsType<NotFoundError>);
        
        #endregion
    }

    [Fact]
    public async Task OrderCreateCommand_Should_CreateOrder_When_PreconditionsMet()
    {
        var mediator = _fixture.GetService<IMediator>();
        
        string customerNumber = Guid.NewGuid().ToString();
        string sessionId      = Guid.NewGuid().ToString();

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

        OrderCreateCommand orderCreateCommand = new(customerNumber, sessionId);
        Result<OrderCreateResponse> orderCreateResult = await mediator.Send(orderCreateCommand);
        
        #endregion
        
        #region Assert
        
        Assert.NotNull(orderCreateResult);
        Assert.True(orderCreateResult.IsOk);

        OrderCreateResponse response = orderCreateResult.UnwrapOrDefault();
        Assert.NotNull(response);
        Assert.NotEqual(default, response.OrderId);

        var events = await _fixture.EventStoreFixture.Events($"{typeof(Order).FullName}-{response.OrderId}");
        Assert.NotEmpty(events);
        Assert.Contains(events, e => e is OrderCreatedEvent);

        var orderCreatedEvent = events.SingleOrDefault(e => e is OrderCreatedEvent) as OrderCreatedEvent;
        Assert.NotNull(orderCreatedEvent);
        Assert.Equal(response.OrderId, orderCreatedEvent.OrderId);
        Assert.Equal(customerNumber, orderCreatedEvent.CustomerNumber);
        
        var orderLinesCatalogueNumbers = orderCreatedEvent.OrderLines.Select(ol => ol.CatalogueNumber).ToArray();
        Assert.All(productNumbers, pn => orderLinesCatalogueNumbers.Contains(pn));

        #endregion
    }

    [Fact]
    public async Task OrderSubmitPaymentCommand_Should_SubmitPayment()
    {
        var mediator = _fixture.GetService<IMediator>();

        #region Preconditions
        
        string customerNumber = Guid.NewGuid().ToString();
        string sessionId      = Guid.NewGuid().ToString();

        Guid paymentId = Guid.NewGuid();
        
        string[] productNumbers =
        {
            Guid.NewGuid().ToString(), 
            Guid.NewGuid().ToString(), 
            Guid.NewGuid().ToString()
        };
        
        await _fixture.ProductsExist(
            new ProductEntity { CatalogueNumber = productNumbers[0], Available = true, Name = "Product0" },
            new ProductEntity { CatalogueNumber = productNumbers[1], Available = true, Name = "Product1" },
            new ProductEntity { CatalogueNumber = productNumbers[2], Available = true, Name = "Product2" });
        await _fixture.BuyerCreated(customerNumber, sessionId, productNumbers);
        Guid orderId = await _fixture.OrderExists(customerNumber, sessionId);

        #endregion

        #region Act

        OrderSubmitPaymentCommand command = new(orderId, paymentId);
        Result result = await mediator.Send(command);

        #endregion

        #region Assert
        
        Assert.NotNull(result);
        Assert.True(result.IsOk);

        // TODO
        
        #endregion
    }

    [Fact]
    public async Task OrderSubmitPaymentCommand_Should_ReturnError_When_PaymentAlreadySubmitted()
    {
        var mediator = _fixture.GetService<IMediator>();

        #region Preconditions
        
        string customerNumber = Guid.NewGuid().ToString();
        string sessionId      = Guid.NewGuid().ToString();

        Guid paymentId = Guid.NewGuid();
        
        string[] productNumbers =
        {
            Guid.NewGuid().ToString(), 
            Guid.NewGuid().ToString(), 
            Guid.NewGuid().ToString()
        };
        
        await _fixture.ProductsExist(
            new ProductEntity { CatalogueNumber = productNumbers[0], Available = true, Name = "Product0" },
            new ProductEntity { CatalogueNumber = productNumbers[1], Available = true, Name = "Product1" },
            new ProductEntity { CatalogueNumber = productNumbers[2], Available = true, Name = "Product2" });
        await _fixture.BuyerCreated(customerNumber, sessionId, productNumbers);
        Guid orderId = await _fixture.OrderExists(customerNumber, sessionId);
        
        OrderSubmitPaymentCommand submitPaymentCommand = new(orderId, paymentId);
        await mediator.Send(submitPaymentCommand);

        #endregion

        #region Act

        OrderSubmitPaymentCommand command = new(orderId, paymentId);
        Result result = await mediator.Send(command);

        #endregion

        #region Assert
        
        Assert.NotNull(result);
        Assert.True(result.IsError);

        // TODO
        
        #endregion
    }
    
    [Fact]
    public async Task OrderSetShippingInformationCommand_Should_SetShippingInfo() 
    {
        var mediator = _fixture.GetService<IMediator>();

        #region Preconditions
        
        string customerNumber = Guid.NewGuid().ToString();
        string sessionId      = Guid.NewGuid().ToString();
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
        
        await _fixture.ProductsExist(
            new ProductEntity { CatalogueNumber = productNumbers[0], Available = true, Name = "Product0" },
            new ProductEntity { CatalogueNumber = productNumbers[1], Available = true, Name = "Product1" },
            new ProductEntity { CatalogueNumber = productNumbers[2], Available = true, Name = "Product2" });
        await _fixture.BuyerCreated(customerNumber, sessionId, productNumbers);
        Guid orderId = await _fixture.OrderExists(customerNumber, sessionId);

        #endregion
        
        #region Act

        OrderSetShippingInformationCommand command = new(orderId, shippingInformation);
        Result result = await mediator.Send(command);

        #endregion
        
        #region Assert
        
        Assert.NotNull(result);
        Assert.True(result.IsOk);
        
        var events = await _fixture.EventStoreFixture.Events($"{typeof(Order).FullName}-{orderId}");
        Assert.NotEmpty(events);
        Assert.Contains(events, e => e is OrderShippingInformationSetEvent);

        var orderCreatedEvent = events.SingleOrDefault(e => e is OrderShippingInformationSetEvent) as OrderShippingInformationSetEvent;
        Assert.NotNull(orderCreatedEvent);
        Assert.Equal(orderId, orderCreatedEvent.OrderId);
        
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