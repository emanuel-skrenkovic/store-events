using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Order.Application.Buyer.Commands.RemoveItemFromCart
{
    public record BuyerRemoveItemFromCartCommand(string CustomerNumber, string SessionId, string ItemCatalogueNumber) : IRequest<Result>;
}