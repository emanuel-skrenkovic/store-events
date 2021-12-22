using System.Threading.Tasks;
using Store.Shopping.Domain.Buyers.ValueObjects;

namespace Store.Shopping.Domain.Buyers;

public interface IBuyerRepository
{
    Task<Buyer> GetBuyerAsync(BuyerIdentifier buyerId);
        
    Task SaveBuyerAsync(Buyer buyer);
}