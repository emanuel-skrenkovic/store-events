using System.Collections.Generic;

namespace Store.Core.Domain.Event
{
    public interface IEventSubscriberProvider
    {
        IEnumerable<IEventSubscriber<TEvent>> GetSubscribers<TEvent>() 
            where TEvent : IIntegrationEvent;
    }
}