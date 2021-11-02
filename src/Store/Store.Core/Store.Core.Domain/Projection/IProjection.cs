namespace Store.Core.Domain.Projection
{
    public interface IProjection<T>
    { 
        T Project(T initialState, object receivedEvent);
    }
}