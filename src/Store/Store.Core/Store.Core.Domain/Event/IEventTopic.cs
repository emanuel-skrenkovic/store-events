using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public interface IEventTopic
    {
        Task StartListeningAsync();

        Task StopListeningAsync();
    }
}