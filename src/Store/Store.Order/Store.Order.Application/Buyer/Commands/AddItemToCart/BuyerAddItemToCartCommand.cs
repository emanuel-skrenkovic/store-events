using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Order.Application.Buyer.Commands.AddItemToCart;

public record BuyerAddItemToCartCommand(string CustomerNumber, string SessionId, string ItemCatalogueNumber) : IRequest<Result>;