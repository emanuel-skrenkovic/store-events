using System.Collections.Generic;

namespace Store.Core.Domain.Event.Integration
{
    public interface IEventSubscriberProvider
    {
        IEnumerable<IEventSubscriber<TEvent>> GetSubscribers<TEvent>() where TEvent : IEvent;
    }
}