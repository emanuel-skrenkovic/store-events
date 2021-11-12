using MediatR;

namespace Store.Order.Application.Buyer.Queries.GetCart
{
    public record BuyerCartGetQuery(string CustomerNumber) : IRequest<CartView>; // TODO: session id
}