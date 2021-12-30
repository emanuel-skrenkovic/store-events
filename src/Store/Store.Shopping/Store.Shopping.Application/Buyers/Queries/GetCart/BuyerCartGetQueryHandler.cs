using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Buyers.Queries.GetCart;

public class BuyerCartGetQueryHandler : IRequestHandler<BuyerCartGetQuery, Result<Cart>>
{
    private readonly CartReadService _cartReadService;

    public BuyerCartGetQueryHandler(CartReadService cartReadService)
        => _cartReadService = cartReadService ?? throw new ArgumentNullException(nameof(cartReadService));

    public Task<Result<Cart>> Handle(BuyerCartGetQuery request, CancellationToken cancellationToken)
        => _cartReadService.GetCartAsync(request.BuyerId);
}