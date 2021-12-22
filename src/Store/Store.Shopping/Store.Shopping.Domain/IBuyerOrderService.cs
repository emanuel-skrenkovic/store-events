using System.Threading.Tasks;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Orders;

namespace Store.Shopping.Domain;

public interface IBuyerOrderService
{
    Task<Result<Order>> PlaceOrderAsync(Buyer buyer);
}