using System.Threading.Tasks;

namespace Store.Core.Domain
{
    public interface ISubscriptionCheckpointRepository
    {
        Task<ulong?> GetAsync(string id); // Generic id?

        Task SaveAsync(string id, ulong position);
    }
}