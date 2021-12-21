using System.Threading.Tasks;
using Store.Order.Domain.Buyers.ValueObjects;

namespace Store.Order.Domain.Buyers;

public interface IBuyerRepository
{
    Task<Buyer> GetBuyerAsync(BuyerIdentifier buyerId);
        
    Task SaveBuyerAsync(Buyer buyer);
}