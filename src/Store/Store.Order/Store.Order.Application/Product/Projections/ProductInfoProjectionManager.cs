using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure.EntityFramework.Extensions;
using Store.Order.Infrastructure;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Product.Projections
{
    public class ProductInfoProjectionManager : IProjectionManager
    {
         private const string SubscriptionId = nameof(ProductInfoEntity);
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IProjection<ProductInfoEntity, StoreOrderDbContext> _projection;
        private readonly IEventSubscriptionFactory _eventSubscriptionFactory;

        public ProductInfoProjectionManager(
            IServiceScopeFactory scopeFactory,
            IProjection<ProductInfoEntity, StoreOrderDbContext> projection,
            IEventSubscriptionFactory eventSubscriptionFactory)
        {
            _scopeFactory             = scopeFactory             ?? throw new ArgumentNullException(nameof(scopeFactory));
            _projection               = projection               ?? throw new ArgumentNullException(nameof(projection));
            _eventSubscriptionFactory = eventSubscriptionFactory ?? throw new ArgumentNullException(nameof(eventSubscriptionFactory));
        }
        
        public async Task StartAsync()
        {
            using IServiceScope scope = _scopeFactory.CreateScope();

            StoreOrderDbContext context = scope.ServiceProvider.GetRequiredService<StoreOrderDbContext>();

            if (context == null)
            {
                throw new InvalidOperationException($"Context cannot be null on {nameof(ProductInfoProjectionManager)} startup.");
            }
            
            ulong checkpoint = await context.GetSubscriptionCheckpoint(SubscriptionId);
            
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
            Ensure.NotNull(@event, nameof(@event));

            using IServiceScope scope = _scopeFactory.CreateScope();

            StoreOrderDbContext context = scope.ServiceProvider.GetRequiredService<StoreOrderDbContext>();
            if (context == null) return;

            Func<Task> projectionAction = _projection.Project(@event, context);
            if (projectionAction == null) return;

            await projectionAction();
            await context.AddOrUpdateSubscriptionCheckpoint(SubscriptionId, eventMetadata.StreamPosition);

            await context.SaveChangesAsync();
        }
    }
}