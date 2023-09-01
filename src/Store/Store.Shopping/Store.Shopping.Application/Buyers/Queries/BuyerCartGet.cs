using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Buyers.Queries;

public record BuyerCartGetQuery(BuyerIdentifier BuyerId) : IRequest<Result<Cart>>; // TODO: session id

public class BuyerCartGet : IRequestHandler<BuyerCartGetQuery, Result<Cart>>
{
    private readonly CartReadService _cartReadService;

    public BuyerCartGet(CartReadService cartReadService)
        => _cartReadService = cartReadService ?? throw new ArgumentNullException(nameof(cartReadService));

    public Task<Result<Cart>> Handle(BuyerCartGetQuery request, CancellationToken cancellationToken)
        => _cartReadService.GetCartAsync(request.BuyerId);
}