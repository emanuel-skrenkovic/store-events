using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Integration.Events;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EntityFramework.Extensions;
using Store.Order.Infrastructure;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Product.Projections
{
    public class ProductProjection : IEventListener
    {
         private const string SubscriptionId = nameof(ProductEntity);
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEventSubscriptionFactory _eventSubscriptionFactory;

        public ProductProjection(
            IServiceScopeFactory scopeFactory,
            IEventSubscriptionFactory eventSubscriptionFactory)
        {
            _scopeFactory             = scopeFactory             ?? throw new ArgumentNullException(nameof(scopeFactory));
            _eventSubscriptionFactory = eventSubscriptionFactory ?? throw new ArgumentNullException(nameof(eventSubscriptionFactory));
        }
        
        public async Task StartAsync()
        {
            using IServiceScope scope = _scopeFactory.CreateScope();

            StoreOrderDbContext context = scope.ServiceProvider.GetRequiredService<StoreOrderDbContext>();

            if (context == null)
            {
                throw new InvalidOperationException($"Context cannot be null on {nameof(ProductProjection)} startup.");
            }
            
            ulong checkpoint = await context.GetSubscriptionCheckpoint(SubscriptionId);
            
            await _eventSubscriptionFactory
                .Create(SubscriptionId, ProjectAsync)
                .SubscribeAtAsync(checkpoint);
        }

        public Task StopAsync()
        {
            // TODO: nothing needed
            return Task.CompletedTask;
        }
        
        private async Task ProjectAsync(IEvent receivedEvent, EventMetadata eventMetadata)
        {
            Ensure.NotNull(receivedEvent, nameof(receivedEvent));

            using IServiceScope scope = _scopeFactory.CreateScope();

            StoreOrderDbContext context = scope.ServiceProvider.GetRequiredService<StoreOrderDbContext>();
            if (context == null) return;

            Func<Task> projectionAction = receivedEvent switch
            {
                ProductAddedEvent @event        => () => When(@event, context),
                ProductPriceChangedEvent @event => () => When(@event, context),
                ProductRenamedEvent @event      => () => When(@event, context),
                ProductAvailableEvent @event    => () => When(@event, context),
                ProductUnavailableEvent @event  => () => When(@event, context),
                _ => null
            };
            if (projectionAction == null) return;

            await projectionAction();
            await context.AddOrUpdateSubscriptionCheckpoint(SubscriptionId, eventMetadata.StreamPosition);

            await context.SaveChangesAsync();
        }
        
        private Task When(ProductAddedEvent @event, StoreOrderDbContext context)
        {
            ProductEntity productEntity = new()
            {
                CatalogueNumber = @event.ProductId.ToString(),
                Name = @event.Name,
                Price = @event.Price
            };
            
            context.Add(productEntity);

            return Task.CompletedTask;
        }
        
        private async Task When(ProductPriceChangedEvent @event, StoreOrderDbContext context)
        {
            ProductEntity productEntity = await context.FindAsync<ProductEntity>(@event.ProductId);
            if (productEntity == null) return;

            productEntity.Price = @event.NewPrice;
            
            context.Update(productEntity);
        }
        
        private async Task When(ProductRenamedEvent @event, StoreOrderDbContext context)
        {
            ProductEntity productEntity = await context.FindAsync<ProductEntity>(@event.ProductId);
            if (productEntity == null) return;

            productEntity.Name = @event.NewName;
            
            context.Update(productEntity);
        }
        
        private async Task When(ProductAvailableEvent @event, StoreOrderDbContext context)
        {
            ProductEntity productEntity = await context.FindAsync<ProductEntity>(@event.ProductId);
            if (productEntity == null) return;
            
            productEntity.Available = true;
            
            context.Update(productEntity);
        }
        
        private async Task When(ProductUnavailableEvent @event, StoreOrderDbContext context)
        {
            ProductEntity productEntity = await context.FindAsync<ProductEntity>(@event.ProductId);
            if (productEntity == null) return;

            productEntity.Available = false;
            
            context.Update(productEntity);
        }
    }
}