using System.Threading.Tasks;

namespace Store.Order.Domain.Buyers
{
    public interface IBuyerRepository
    {
        Task<Buyer> GetBuyerAsync(string customerNumber);
        
        Task SaveBuyerAsync(Buyer buyer);
    }
}