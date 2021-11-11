using System;
using System.Threading.Tasks;
using Store.Core.Domain;

namespace Store.Catalogue.Domain.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly IAggregateRepository _repository;
        
        public ProductRepository(IAggregateRepository aggregateRepository)
        {
            _repository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
        }
        
        public Task<Product> GetProductAsync(Guid id)
        {
            return _repository.GetAsync<Product, Guid>(id);
        }

        public Task SaveProductAsync(Product product)
        {
            Guard.IsNotNull(product, nameof(product));
            
            return _repository.SaveAsync<Product, Guid>(product);
        }
    }
}