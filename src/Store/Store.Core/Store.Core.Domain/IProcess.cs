using Store.Core.Domain.Event;

namespace Store.Core.Domain;

public interface IProcess<TState>
{
    bool TryNext(
        TState currentState, 
        IEvent receivedEvent, 
        out TState updatedState, 
        out object command);
}