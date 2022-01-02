using System.Data;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Orders.ValueObjects;
using Store.Shopping.Domain.Payments.ValueObjects;
using Store.Shopping.Domain.ValueObjects;
using Store.Shopping.Infrastructure;
using Store.Shopping.Infrastructure.Entity;
using Order = Store.Shopping.Domain.Orders.Order;
using OrderLine = Store.Shopping.Domain.Orders.OrderLine;
using ShippingInformation = Store.Shopping.Domain.Orders.ValueObjects.ShippingInformation;

namespace Store.Shopping.Application.Orders.Commands.PlaceOrder;

public class OrderPlaceCommandHandler : IRequestHandler<OrderPlaceCommand, Result<OrderPlaceResponse>>
{
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IDbConnection _db;

    public OrderPlaceCommandHandler(
        IBuyerRepository buyerRepository, 
        IOrderRepository orderRepository,
        StoreShoppingDbContext context)
    {
        _buyerRepository = buyerRepository                     ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderRepository = orderRepository                     ?? throw new ArgumentNullException(nameof(orderRepository));
        _db              = context?.Database.GetDbConnection() ?? throw new ArgumentNullException(nameof(context));
    }
        
    public Task<Result<OrderPlaceResponse>> Handle(OrderPlaceCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        (string customerNumber, 
         string sessionId, 
         Guid paymentNumber, 
         ShippingInfo shippingInfo) = request;
        
        BuyerIdentifier buyerId = new(customerNumber, sessionId);
        OrderNumber orderNumber = new(Guid.NewGuid());
        
        return _buyerRepository.GetBuyerAsync(buyerId)
            .Then(buyer => ValidateShippingInfo(shippingInfo).Then(si => CreateOrder(buyer,orderNumber, new(paymentNumber), si)))
            .Then<OrderPlaceResponse>(() => new OrderPlaceResponse(orderNumber.Value));
    }

    private async Task<Result> CreateOrder(
        Buyer buyer, 
        OrderNumber orderNumber, 
        PaymentNumber paymentNumber, 
        ShippingInformation shippingInformation)
    {
        string[] catalogueNumbers = buyer
            .CartItems
            .Select(kv => kv.Key)
            .ToArray();

        if (!catalogueNumbers.Any()) return new Error($"Cannot create order out of an empty cart. BuyerId {buyer.Id}");

        string query =
            @"SELECT p.catalogue_number AS CatalogueNumber,
                     p.name,
                     p.price,
                     p.available
              FROM public.product p
              WHERE p.catalogue_number = ANY(@catalogueNumbers);";

        IEnumerable<ProductInfo> productsInfo = await _db.QueryAsync<ProductInfo>(query, new { catalogueNumbers });

        OrderLines orderLines = new(productsInfo.Select(i =>
        {
            uint count = buyer.CartItems[i.CatalogueNumber];
            return new OrderLine(
                i.CatalogueNumber,
                count * i.Price,
                count);
        }).ToArray());
            
        Order order = Order.Create(
            orderNumber,
            new CustomerNumber(buyer.CustomerNumber),
            paymentNumber,
            orderLines,
            shippingInformation);

        return await _orderRepository.SaveOrderAsync(order); 
    }
    
    private Result<ShippingInformation> ValidateShippingInfo(ShippingInfo shippingInfo) 
        => ShippingInformation.Create(
            shippingInfo.CountryCode, 
            shippingInfo.FullName, 
            shippingInfo.StreetAddress,
            shippingInfo.City,
            shippingInfo.StateProvince, 
            shippingInfo.Postcode, 
            shippingInfo.PhoneNumber);
}