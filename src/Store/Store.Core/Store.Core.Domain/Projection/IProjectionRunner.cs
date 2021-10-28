using System.Threading.Tasks;

namespace Store.Core.Domain.Projection
{
    // TODO: should probably invert IProjection and IProjectionRunner.
    // Have IProjection return the updated model, and runner update the DB with it.
    public interface IProjectionRunner
    {
        Task RunAsync<T>(IProjection<T> projection, object @event) where T : class, new();
    }
}