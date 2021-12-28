using System.Threading.Tasks;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Buyers.ValueObjects;

namespace Store.Shopping.Domain.Buyers;

public interface IBuyerRepository
{
    Task<Result<Buyer>> GetBuyerAsync(BuyerIdentifier buyerId);
        
    Task<Result> SaveBuyerAsync(Buyer buyer);
}