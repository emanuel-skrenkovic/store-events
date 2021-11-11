using System.Threading.Tasks;
using Store.Core.Domain.Functional;
using Store.Core.Domain.Result;

namespace Store.Order.Domain.Buyers
{
    public interface IBuyerRepository
    {
        Task<Buyer> GetBuyerAsync(string customerNumber);
        
        Task<Result<Unit>> CreateBuyerAsync(Buyer buyer);

        Task<Result<Unit>> SaveBuyerAsync(Buyer buyer);
    }
}