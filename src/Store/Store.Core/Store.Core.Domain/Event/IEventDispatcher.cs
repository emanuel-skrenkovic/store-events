using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(object @event);
    }
}