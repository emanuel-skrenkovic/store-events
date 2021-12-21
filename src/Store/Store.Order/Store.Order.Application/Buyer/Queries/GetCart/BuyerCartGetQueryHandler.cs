using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Buyer.Queries.GetCart;

public class BuyerCartGetQueryHandler : IRequestHandler<BuyerCartGetQuery, Result<Cart>>
{
    private readonly CartReadService _cartReadService;

    public BuyerCartGetQueryHandler(CartReadService cartReadService)
        => _cartReadService = cartReadService ?? throw new ArgumentNullException(nameof(cartReadService));

    public async Task<Result<Cart>> Handle(BuyerCartGetQuery request, CancellationToken cancellationToken)
    { 
        Cart cart = await _cartReadService.GetCartAsync(request.BuyerId);

        if (cart == null) return new NotFoundError("Buyer's cart was not found.");

        return cart;
    }
}