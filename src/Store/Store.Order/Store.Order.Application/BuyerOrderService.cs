using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Application.Product;
using Store.Order.Domain;
using Store.Order.Domain.Orders.ValueObjects;
using Store.Order.Domain.ValueObjects;
using Store.Order.Infrastructure.Entity;
using OrderLine = Store.Order.Domain.Orders.OrderLine;

namespace Store.Order.Application;

public class BuyerOrderService : IBuyerOrderService
{
    private readonly ProductInfoService _productInfoService;

    public BuyerOrderService(ProductInfoService productInfoService)
        => _productInfoService = productInfoService ?? throw new ArgumentNullException(nameof(productInfoService));
        
    public async Task<Result<Domain.Orders.Order>> PlaceOrderAsync(Domain.Buyers.Buyer buyer)
    {
        Ensure.NotNull(buyer, nameof(buyer));

        IEnumerable<ProductInfo> productsInfo = await _productInfoService.GetProductsInfoAsync(buyer
            .CartItems
            .Select(kv => kv.Key)
            .ToArray());

        OrderLines orderLines = new(productsInfo.Select(i =>
        {
            uint count = buyer.CartItems[i.CatalogueNumber];
            return new OrderLine(
                i.CatalogueNumber,
                count * i.Price,
                count);
        }).ToArray());

        return Domain.Orders.Order.Create(
            new OrderNumber(Guid.NewGuid()), 
            new CustomerNumber(buyer.CustomerNumber), 
            orderLines);
    }
}