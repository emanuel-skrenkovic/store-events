using System.Threading.Tasks;

namespace Store.Core.Domain
{
    public interface ICheckpointRepository
    {
        Task<ulong> GetAsync(string subscriptionId); // Generic id?

        Task SaveAsync(string subscriptionId, ulong position);
    }
}