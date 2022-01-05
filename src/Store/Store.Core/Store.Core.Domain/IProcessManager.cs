using System.Threading.Tasks;
using Store.Core.Domain.Event;

namespace Store.Core.Domain;

public interface IProcessManager<TState>
{
    Task HandleAsync(IProcess<TState> process, IEvent @event, EventMetadata eventMetadata);
}