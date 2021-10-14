using System;
using System.Threading.Tasks;

namespace Store.Catalogue.Domain.Product
{
    public interface IProductRepository
    {
        Task<Product> GetProductAsync(Guid id);

        Task SaveProductAsync(Product product);
    }
}