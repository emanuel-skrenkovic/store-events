using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.ValueObjects;

namespace Store.Shopping.Application.Buyers.Commands.RemoveItemFromCart;

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
        
        Buyer buyer = await _buyerRepository.GetBuyerAsync(buyerId);
        if (buyer == null) return new NotFoundError($"Entity with id {buyerId.CustomerNumber} not found.");
            
        buyer.RemoveCartItem(new CatalogueNumber(request.ItemCatalogueNumber));

        await _buyerRepository.SaveBuyerAsync(buyer);

        return Result.Ok();
    }
}