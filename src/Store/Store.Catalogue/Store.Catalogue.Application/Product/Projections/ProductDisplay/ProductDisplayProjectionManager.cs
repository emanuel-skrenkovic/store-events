using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public class ProductDisplayProjectionManager : IProjectionManager
    {
        private const string SubscriptionId = nameof(ProductDisplayEntity);
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IProjection<ProductDisplayEntity, StoreCatalogueDbContext> _projection;
        private readonly IEventSubscriptionFactory _eventSubscriptionFactory;
        private readonly ICheckpointRepository _checkpointRepository;

        public ProductDisplayProjectionManager(
            IServiceScopeFactory scopeFactory,
            IProjection<ProductDisplayEntity, StoreCatalogueDbContext> projection,
            IEventSubscriptionFactory eventSubscriptionFactory,
            ICheckpointRepository checkpointRepository)
        {
            _scopeFactory             = scopeFactory             ?? throw new ArgumentNullException(nameof(scopeFactory));
            _projection               = projection               ?? throw new ArgumentNullException(nameof(projection));
            _eventSubscriptionFactory = eventSubscriptionFactory ?? throw new ArgumentNullException(nameof(eventSubscriptionFactory));
            _checkpointRepository     = checkpointRepository     ?? throw new ArgumentNullException(nameof(checkpointRepository));
        }
        
        public async Task StartAsync()
        {
            ulong checkpoint = await _checkpointRepository.GetAsync(nameof(ProductDisplayEntity));
            await _eventSubscriptionFactory
                .Create(SubscriptionId, ProjectEventAsync)
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

            Func<Task> projectionAction = _projection.Project(@event, context);
            if (projectionAction == null) return;

            // TODO: think about ref model parameter
            await projectionAction();
            // TODO: should be in the same unit of work as the projection changes.
            await _checkpointRepository.SaveAsync(SubscriptionId, eventMetadata.StreamPosition);
            
            await context.SaveChangesAsync();
        }
    }
}