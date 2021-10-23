using System;
using System.Threading.Tasks;
using Store.Core.Domain;
using Store.Core.Infrastructure;

namespace Store.Catalogue.Domain.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly IRepository _repository;
        
        public ProductRepository(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public Task<Product> GetProductAsync(Guid id)
        {
            return _repository.GetAsync<Product>(id);
        }

        public Task CreateProductAsync(Product product)
        {
            return _repository.CreateAsync(product);
        }

        public Task SaveProductAsync(Product product)
        {
            Guard.IsNotNull(product, nameof(product));
            
            return _repository.SaveAsync(product);
        }
    }
}