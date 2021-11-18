using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public interface IEventListener
    {
        Task StartAsync();

        Task StopAsync();
    }
}