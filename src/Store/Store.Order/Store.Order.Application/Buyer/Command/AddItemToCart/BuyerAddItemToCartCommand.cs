using MediatR;
using Store.Core.Domain.Result;
using Store.Order.Domain;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Buyer.Command.AddItemToCart
{
    public record BuyerAddItemToCartCommand(string CustomerNumber, Item Item) : IRequest<Result<Unit>>;
}