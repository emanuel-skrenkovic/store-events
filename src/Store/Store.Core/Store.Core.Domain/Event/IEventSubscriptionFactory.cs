using System;
using System.Threading.Tasks;

namespace Store.Core.Domain.Event;

public interface IEventSubscriptionFactory
{
    IEventSubscription Create(string subscriptionId, Func<IEvent, EventMetadata, Task> handleEvent);
}