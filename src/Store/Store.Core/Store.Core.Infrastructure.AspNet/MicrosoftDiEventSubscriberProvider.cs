using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain.Event;

namespace Store.Core.Infrastructure.AspNet
{
    // Shitty service locator implementation. Don't like it.
    public class MicrosoftDiEventSubscriberProvider : IEventSubscriberProvider
    {
        private readonly IServiceCollection _services;
        
        public MicrosoftDiEventSubscriberProvider(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }
        
        public IEnumerable<IEventSubscriber<TEvent>> GetSubscribers<TEvent>() where TEvent : IIntegrationEvent
        {
            return _services
                .Where(s => s.ImplementationType == typeof(IEventSubscriber<TEvent>))
                .Select(s => (IEventSubscriber<TEvent>)s.ImplementationInstance);
        }
    }
}