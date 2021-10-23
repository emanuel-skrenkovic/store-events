using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain.Event;

namespace Store.Core.Infrastructure.AspNet
{
    // Shitty service locator implementation. Don't like it.
    public class MicrosoftDiEventSubscriberProvider : IEventSubscriberProvider
    {
        private readonly IServiceProvider _serviceProvider;
        
        public MicrosoftDiEventSubscriberProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        
        public IEnumerable<IEventSubscriber<TEvent>> GetSubscribers<TEvent>() where TEvent : IIntegrationEvent
        {
            return _serviceProvider.GetServices<IEventSubscriber<TEvent>>();
        }
    }
}