using System.Threading.Tasks;

namespace Store.Core.Domain.Projection
{
    public interface IProjectionManager
    {
        Task StartAsync();

        Task StopAsync();
    }
}