using System.Data;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Infrastructure;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Orders.Queries;

public record GetOrderQuery(Guid OrderId) : IRequest<Result<Order>>;

public class GetOrder : IRequestHandler<GetOrderQuery, Result<Order>>
{
    private readonly ISerializer _serializer;
    private readonly IDbConnection _db;

    public GetOrder(ISerializer serializer, StoreShoppingDbContext context)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _db = context?.Database.GetDbConnection() ?? throw new ArgumentNullException(nameof(context));   
    }
    
    public async Task<Result<Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        const string query = 
            @"SELECT o.Data
              FROM public.order o
              WHERE o.order_id = @OrderId;";

        var orderEntity = await _db.QuerySingleOrDefaultAsync<OrderEntity>(query, new { request.OrderId });
        if (orderEntity == null) return new NotFoundError($"Order with {request.OrderId} was not found.");

        return _serializer.Deserialize<Order>(orderEntity.Data);
    }
}