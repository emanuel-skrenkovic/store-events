using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.ValueObjects;

namespace Store.Shopping.Application.Buyers.Commands;

public record BuyerRemoveItemFromCartCommand(string CustomerNumber, string SessionId, string ProductCatalogueNumber) : IRequest<Result>;

public class BuyerRemoveItemFromCart : IRequestHandler<BuyerRemoveItemFromCartCommand, Result>
{
    private readonly IAggregateRepository _repository;

    public BuyerRemoveItemFromCart(IAggregateRepository repository)
        => _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        
    public Task<Result> Handle(BuyerRemoveItemFromCartCommand request, CancellationToken ct)
    {
        (string customerNumber, string sessionId, string productCatalogueNumber) = request;
        BuyerIdentifier buyerId = new(customerNumber, sessionId);
        
        return _repository.GetAsync<Buyer, string>(buyerId.ToString(), ct)
            .Then(buyer =>
            {
                buyer.RemoveCartItem(new CatalogueNumber(productCatalogueNumber));
                return _repository.SaveAsync<Buyer, string>(buyer, ct);
            });
    }
}