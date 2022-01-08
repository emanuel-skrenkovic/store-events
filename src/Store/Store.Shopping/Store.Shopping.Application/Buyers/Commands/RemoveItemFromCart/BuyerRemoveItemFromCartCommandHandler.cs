using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.ValueObjects;

namespace Store.Shopping.Application.Buyers.Commands.RemoveItemFromCart;

public class BuyerRemoveItemFromCartCommandHandler : IRequestHandler<BuyerRemoveItemFromCartCommand, Result>
{
    private readonly IAggregateRepository _repository;

    public BuyerRemoveItemFromCartCommandHandler(IAggregateRepository repository)
        => _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        
    public Task<Result> Handle(BuyerRemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        (string customerNumber, string sessionId, string productCatalogueNumber) = request;
        BuyerIdentifier buyerId = new(customerNumber, sessionId);
        
        return _repository.GetAsync<Buyer, string>(buyerId.ToString())
            .Then(buyer =>
            {
                buyer.RemoveCartItem(new CatalogueNumber(productCatalogueNumber));
                return _repository.SaveAsync<Buyer, string>(buyer);
            });
    }
}