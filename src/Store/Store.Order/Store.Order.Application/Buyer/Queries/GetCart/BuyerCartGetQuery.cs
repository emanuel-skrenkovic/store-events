using MediatR;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Buyer.Queries.GetCart
{
    public record BuyerCartGetQuery(string CustomerNumber) : IRequest<Cart>; // TODO: session id
}