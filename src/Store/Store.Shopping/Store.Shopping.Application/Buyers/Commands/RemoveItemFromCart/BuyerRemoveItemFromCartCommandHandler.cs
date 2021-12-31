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
        => _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        
    public Task<Result> Handle(BuyerRemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        (string customerNumber, string sessionId, string itemCatalogueNumber) = request;
        BuyerIdentifier buyerId = new(customerNumber, sessionId);
        
        return _buyerRepository.GetBuyerAsync(buyerId).Then(buyer =>
        {
            buyer.RemoveCartItem(new CatalogueNumber(itemCatalogueNumber));
            return _buyerRepository.SaveBuyerAsync(buyer); 
        });
    }
}