using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.ValueObjects;

namespace Store.Shopping.Application.Buyers.Commands.AddItemToCart;

public class BuyerAddItemToCartCommandHandler : IRequestHandler<BuyerAddItemToCartCommand, Result>
{
    private readonly IBuyerRepository _buyerRepository;

    public BuyerAddItemToCartCommandHandler(IBuyerRepository buyerRepository)
    {
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
    }
        
    public async Task<Result> Handle(BuyerAddItemToCartCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        BuyerIdentifier buyerId = new(request.CustomerNumber, request.SessionId);
        Buyer buyer = await _buyerRepository.GetBuyerAsync(buyerId) ?? Buyer.Create(buyerId);
            
        // TODO: check if warning correct.
        buyer.AddCartItem(new CatalogueNumber(request.ItemCatalogueNumber));

        await _buyerRepository.SaveBuyerAsync(buyer);

        return Result.Ok();
    }
}