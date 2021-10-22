using System.Threading.Tasks;

namespace Store.Core.Domain.Event
{
    public interface IEventPublisher
    {
        Task PublishAsync(IIntegrationEvent integrationEvent);
    }
}