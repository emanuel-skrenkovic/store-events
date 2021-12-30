using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Orders.Queries;

public record GetOrderQuery(Guid OrderId) : IRequest<Result<Order>>;