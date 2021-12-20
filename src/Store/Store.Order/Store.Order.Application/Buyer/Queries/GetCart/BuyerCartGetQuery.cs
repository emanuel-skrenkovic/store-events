using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Domain.Buyers.ValueObjects;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Buyer.Queries.GetCart
{
    public record BuyerCartGetQuery(BuyerIdentifier BuyerId) : IRequest<Result<Cart>>; // TODO: session id
}