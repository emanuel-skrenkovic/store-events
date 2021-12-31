using System;
using System.Threading.Tasks;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Buyers.ValueObjects;

namespace Store.Shopping.Domain.Buyers;

public class BuyerRepository : IBuyerRepository
{
    private readonly IAggregateRepository _repository;

    public BuyerRepository(IAggregateRepository repository)
        => _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        
    public Task<Result<Buyer>> GetBuyerAsync(BuyerIdentifier buyerId)
        => _repository.GetAsync<Buyer, string>(buyerId.ToString());

    public Task<Result> SaveBuyerAsync(Buyer buyer)
        => _repository.SaveAsync<Buyer, string>(buyer);
}