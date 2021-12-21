using System.Threading.Tasks;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Domain.Buyers;

namespace Store.Order.Domain;

public interface IBuyerOrderService
{
    Task<Result<Orders.Order>> PlaceOrderAsync(Buyer buyer);
}