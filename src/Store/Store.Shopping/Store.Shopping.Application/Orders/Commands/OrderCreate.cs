using System.Data;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Store.Shopping.Domain.Orders.ValueObjects;
using Store.Shopping.Domain.ValueObjects;
using Store.Shopping.Infrastructure;
using Store.Shopping.Infrastructure.Entity;
using Order = Store.Shopping.Domain.Orders.Order;
using OrderLine = Store.Shopping.Domain.Orders.OrderLine;

namespace Store.Shopping.Application.Orders.Commands;

public record OrderCreateCommand
(
    string CustomerNumber, 
    string SessionId
) : IRequest<Result<OrderCreateResponse>>;

public record OrderCreateResponse(Guid OrderId);

public class OrderCreate : IRequestHandler<OrderCreateCommand, Result<OrderCreateResponse>>
{
    private readonly IAggregateRepository _repository;
    private readonly IDbConnection _db;

    public OrderCreate(
        IAggregateRepository repository,
        StoreShoppingDbContext context)
    {
        _repository = repository                          ?? throw new ArgumentNullException(nameof(repository));
        _db         = context?.Database.GetDbConnection() ?? throw new ArgumentNullException(nameof(context));
    }
        
    public Task<Result<OrderCreateResponse>> Handle(OrderCreateCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        (string customerNumber, string sessionId) = request;
        OrderNumber orderNumber = new(Guid.NewGuid());
        
        return _repository.GetAsync<Buyer, string>(new BuyerIdentifier(customerNumber, sessionId).ToString())
            .Then(buyer => CreateOrder(buyer, orderNumber))
            .Then(order => _repository.SaveAsync<Order, Guid>(order))
            .Then<OrderCreateResponse>(() => new OrderCreateResponse(orderNumber.Value));
    }

    private async Task<Result<Order>>CreateOrder(
        Buyer buyer, 
        OrderNumber orderNumber)
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

        OrderLines orderLines = new(
            productsInfo.Select(i =>
            {
                uint count = buyer.CartItems[i.CatalogueNumber];
                return new OrderLine(
                    i.CatalogueNumber,
                    count * i.Price,
                    count);
            })
            .ToArray());
            
        return Order.Create(
            orderNumber,
            new CustomerNumber(buyer.CustomerNumber),
            orderLines);
    }
}