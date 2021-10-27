using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Store.Core.Domain.Event;

namespace Store.Core.Infrastructure.AspNet
{
    public class EventTopicHostedService : IHostedService
    {
        private readonly IEventTopic _topic;
        
        public EventTopicHostedService(IEventTopic topic)
        {
            _topic = topic ?? throw new ArgumentNullException(nameof(topic));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _topic.StartListeningAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _topic.StopListeningAsync();
        }
    }
}