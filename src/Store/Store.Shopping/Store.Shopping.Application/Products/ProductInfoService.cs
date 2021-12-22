using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Products;

public class ProductInfoService
{
    public Task<IEnumerable<ProductInfo>> GetProductsInfoAsync(params string[] catalogueNumbers)
    {
        throw new NotImplementedException();
    }
}