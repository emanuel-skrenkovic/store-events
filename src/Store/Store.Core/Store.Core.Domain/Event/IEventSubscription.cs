using System.Threading.Tasks;

namespace Store.Core.Domain.Event;

public interface IEventSubscription
{
    Task SubscribeAtAsync(ulong checkpoint);
}