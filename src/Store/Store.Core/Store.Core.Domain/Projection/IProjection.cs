using System.Threading.Tasks;
using Store.Core.Domain.Event;

namespace Store.Core.Domain.Projection
{
    public interface IProjection
    { 
        Task ProjectAsync(IEvent receivedEvent, EventMetadata eventMetadata);
    }
}