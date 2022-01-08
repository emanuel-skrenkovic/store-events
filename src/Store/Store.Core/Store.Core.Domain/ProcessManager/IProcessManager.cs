using System.Threading.Tasks;
using Store.Core.Domain.Event;

namespace Store.Core.Domain.ProcessManager;

public interface IProcessManager<TState>
{
    Task HandleAsync(IEvent @event, EventMetadata eventMetadata);
}