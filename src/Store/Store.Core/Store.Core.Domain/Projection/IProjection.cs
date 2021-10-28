using System.Threading.Tasks;

namespace Store.Core.Domain.Projection
{
    public interface IProjection
    { 
        Task ProjectAsync(object receivedEvent);
    }
}