using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure.EventStore;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public class ProductDisplayProjectionManager : IProjectionManager
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IProjection<ProductDisplayEntity> _projection;
        private readonly IEventSubscriptionFactory _eventSubscriptionFactory;
        private readonly ICheckpointRepository _checkpointRepository;
        private readonly EventStoreConnectionConfiguration _configuration;

        public ProductDisplayProjectionManager(
            IServiceScopeFactory scopeFactory,
            IProjection<ProductDisplayEntity> projection,
            IEventSubscriptionFactory eventSubscriptionFactory,
            ICheckpointRepository checkpointRepository,
            EventStoreConnectionConfiguration configuration)
        {
            _scopeFactory             = scopeFactory             ?? throw new ArgumentNullException(nameof(scopeFactory));
            _projection               = projection               ?? throw new ArgumentNullException(nameof(projection));
            _eventSubscriptionFactory = eventSubscriptionFactory ?? throw new ArgumentNullException(nameof(eventSubscriptionFactory));
            _checkpointRepository     = checkpointRepository     ?? throw new ArgumentNullException(nameof(checkpointRepository));
            _configuration            = configuration            ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public async Task StartAsync()
        {
            ulong checkpoint = await _checkpointRepository.GetAsync(_configuration.SubscriptionId);
            await _eventSubscriptionFactory
                .Create(nameof(ProductDisplayEntity), ProjectEventAsync)
                .SubscribeAtAsync(checkpoint);
        }

        public Task StopAsync()
        {
            // TODO: nothing needed
            return Task.CompletedTask;
        }
        
        private async Task ProjectEventAsync(IEvent @event, EventMetadata eventMetadata)
        {
            Guard.IsNotNull(@event, nameof(@event));

            using IServiceScope scope = _scopeFactory.CreateScope();

            StoreCatalogueDbContext context = scope.ServiceProvider.GetRequiredService<StoreCatalogueDbContext>();
            if (context == null) return;

            Func<Task> projectionAction = _projection.Project(@event);
            if (projectionAction == null) return;

            // TODO: think about ref model parameter
            await projectionAction();
            // TODO: should be in the same unit of work as the projection changes.
            await _checkpointRepository.SaveAsync(_configuration.SubscriptionId, eventMetadata.StreamPosition);
            
            await context.SaveChangesAsync();
        }
    }
}