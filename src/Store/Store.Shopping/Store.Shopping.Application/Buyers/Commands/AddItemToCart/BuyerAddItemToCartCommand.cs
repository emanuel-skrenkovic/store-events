using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Buyers.Commands.AddItemToCart;

public record BuyerAddItemToCartCommand(string CustomerNumber, string SessionId, string ProductCatalogueNumber) : IRequest<Result>;