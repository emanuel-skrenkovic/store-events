using System;
using System.Threading.Tasks;
using Store.Core.Domain;
using Store.Core.Domain.Functional;
using Store.Core.Domain.Result;

namespace Store.Order.Domain.Buyers
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly IAggregateRepository _repository;

        public BuyerRepository(IAggregateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public Task<Buyer> GetBuyerAsync(string customerNumber)
        {
            return _repository.GetAsync<Buyer, string>(customerNumber);
        }

        public Task<Result<Unit>> CreateBuyerAsync(Buyer buyer)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Unit>> SaveBuyerAsync(Buyer buyer)
        {
            throw new NotImplementedException();
        }
    }
}