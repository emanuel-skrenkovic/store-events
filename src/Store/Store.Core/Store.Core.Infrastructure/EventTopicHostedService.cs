using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Event.Integration;
using Store.Core.Infrastructure.EventStore;

namespace Store.Core.Infrastructure
{
    public class EventTopicHostedService : IHostedService
    {
        private IEventTopic _topic;
        private readonly IServiceScopeFactory _scopeFactory;
        
        public EventTopicHostedService(IServiceScopeFactory scopeFactory)
        {
            // _topic = topic ?? throw new ArgumentNullException(nameof(topic));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                _topic = new EventStoreEventTopic(
                    scope.ServiceProvider.GetRequiredService<EventStoreClient>(),
                    scope.ServiceProvider.GetRequiredService<EventStoreEventTopicConfiguration>(),
                    scope.ServiceProvider.GetRequiredService<ICheckpointRepository>(),
                    scope.ServiceProvider.GetRequiredService<IEventBus>(),
                    scope.ServiceProvider.GetRequiredService<ISerializer>());
                // return _topic.StartListeningAsync();

                return _topic.StartListeningAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _topic = null;
            // return _topic.StopListeningAsync();
            return _topic?.StopListeningAsync();
        }
    }
}