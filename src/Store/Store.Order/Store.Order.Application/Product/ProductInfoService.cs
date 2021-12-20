using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Product
{
    public class ProductInfoService
    {
        public Task<IEnumerable<ProductInfo>> GetProductsInfoAsync(params string[] catalogueNumbers)
        {
            throw new NotImplementedException();
        }
    }
}