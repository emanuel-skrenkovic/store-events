using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Buyers.Queries.GetCart;

public record BuyerCartGetQuery(BuyerIdentifier BuyerId) : IRequest<Result<Cart>>; // TODO: session id