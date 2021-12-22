using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Application.Products;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Orders.ValueObjects;
using Store.Shopping.Domain.ValueObjects;
using Store.Shopping.Infrastructure.Entity;
using OrderLine = Store.Shopping.Domain.Orders.OrderLine;
using Order = Store.Shopping.Domain.Orders.Order;

namespace Store.Shopping.Application;

public class BuyerOrderService : IBuyerOrderService
{
    private readonly ProductInfoService _productInfoService;

    public BuyerOrderService(ProductInfoService productInfoService)
        => _productInfoService = productInfoService ?? throw new ArgumentNullException(nameof(productInfoService));
        
    public async Task<Result<Order>> PlaceOrderAsync(Buyer buyer)
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

        return Order.Create(
            new OrderNumber(Guid.NewGuid()), 
            new CustomerNumber(buyer.CustomerNumber), 
            orderLines);
    }
}