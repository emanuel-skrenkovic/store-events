using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Buyers.Commands.RemoveItemFromCart;

public record BuyerRemoveItemFromCartCommand(string CustomerNumber, string SessionId, string ItemCatalogueNumber) : IRequest<Result>;