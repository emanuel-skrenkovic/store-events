using MediatR;
using Store.Core.Domain.Result;
using Store.Order.Domain;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Buyer.Commands.RemoveItemFromCart
{
    public record BuyerRemoveItemFromCartCommand(string CustomerNumber, Item Item) : IRequest<Result<Unit>>;
}