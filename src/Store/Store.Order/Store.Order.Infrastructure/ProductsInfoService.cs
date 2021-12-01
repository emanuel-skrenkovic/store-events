using System;
using System.Threading.Tasks;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure
{
    public class ProductsInfoService
    {
        Task<ProductInfoEntity> GetProductsInformationAsync(params string[] catalogueNumbers)
        {
            throw new NotImplementedException();
        }
    }
}