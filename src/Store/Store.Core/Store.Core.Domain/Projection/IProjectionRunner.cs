using System.Threading.Tasks;
using Store.Core.Domain.Event;

namespace Store.Core.Domain.Projection
{
    // TODO: should probably invert IProjection and IProjectionRunner.
    // Have IProjection return the updated model, and runner update the DB with it.
    public interface IProjectionRunner<T> where T : class, IReadModel, new()
    {
        Task RunAsync(IProjection<T> projection, IEvent @event);
    }
}