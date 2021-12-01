using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Order.Application.Product
{
    public class ProductInfoService
    {
        public Task<IEnumerable<ProductDto>> GetProductsInfoAsync(params string[] catalogueNumbers)
        {
            throw new NotImplementedException();
        }
    }
}