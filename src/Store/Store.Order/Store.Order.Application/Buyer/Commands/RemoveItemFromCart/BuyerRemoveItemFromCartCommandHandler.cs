using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Domain;
using Store.Order.Domain.Buyers;
using Store.Order.Domain.Buyers.ValueObjects;

namespace Store.Order.Application.Buyer.Commands.RemoveItemFromCart;

public class BuyerRemoveItemFromCartCommandHandler : IRequestHandler<BuyerRemoveItemFromCartCommand, Result>
{
    private readonly IBuyerRepository _buyerRepository;

    public BuyerRemoveItemFromCartCommandHandler(IBuyerRepository buyerRepository)
    {
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
    }
        
    public async Task<Result> Handle(BuyerRemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        BuyerIdentifier buyerId = new(request.CustomerNumber, request.SessionId);
        Domain.Buyers.Buyer buyer = await _buyerRepository.GetBuyerAsync(buyerId);
        if (buyer == null) return new NotFoundError($"Entity with id {buyerId.CustomerNumber} not found.");
            
        buyer.RemoveCartItem(new CatalogueNumber(request.ItemCatalogueNumber));

        await _buyerRepository.SaveBuyerAsync(buyer);

        return Result.Ok();
    }
}