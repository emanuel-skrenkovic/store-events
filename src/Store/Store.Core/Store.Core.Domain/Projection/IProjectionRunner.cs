using System.Threading.Tasks;

namespace Store.Core.Domain.Projection
{
    // TODO: should probably invert IProjection and IProjectionRunner.
    // Have IProjection return the updated model, and runner update the DB with it.
    public interface IProjectionRunner
    {
        Task RunAsync<T>(IProjectionOperation<T> operation) where T : class;

        Task RunUpdateAsync<T>(IProjectionUpdateOperation<T> operation) where T : class;
    }
}