using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Integration.Events;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure.EntityFramework.Extensions;
using Store.Order.Infrastructure;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Product.Projections
{
    public class ProductInfoProjection : IEventListener
    {
         private const string SubscriptionId = nameof(ProductInfoEntity);
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISerializer _serializer;
        private readonly IEventSubscriptionFactory _eventSubscriptionFactory;

        public ProductInfoProjection(
            IServiceScopeFactory scopeFactory,
            ISerializer serializer,
            IEventSubscriptionFactory eventSubscriptionFactory)
        {
            _scopeFactory             = scopeFactory             ?? throw new ArgumentNullException(nameof(scopeFactory));
            _serializer               = serializer               ?? throw new ArgumentNullException(nameof(serializer));
            _eventSubscriptionFactory = eventSubscriptionFactory ?? throw new ArgumentNullException(nameof(eventSubscriptionFactory));
        }
        
        public async Task StartAsync()
        {
            using IServiceScope scope = _scopeFactory.CreateScope();

            StoreOrderDbContext context = scope.ServiceProvider.GetRequiredService<StoreOrderDbContext>();

            if (context == null)
            {
                throw new InvalidOperationException($"Context cannot be null on {nameof(ProductInfoProjection)} startup.");
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
        
        public async Task ProjectAsync(IEvent receivedEvent, EventMetadata eventMetadata)
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
            ProductInfoEntity product = new()
            {
                Id = @event.ProductId,
                Name = @event.Name,
                Price = @event.Price
            };
            
            context.AddProjectionDocument(_serializer, product);

            return Task.CompletedTask;
        }
        
        private async Task When(ProductPriceChangedEvent @event, StoreOrderDbContext context)
        {
            ProductInfoEntity product =
                await context.GetProjectionDocumentAsync<ProductInfoEntity>(_serializer, @event.ProductId);
            if (product == null) return;

            product.Price = @event.NewPrice;
            context.UpdateProjectionDocument(_serializer, product);
        }
        
        private async Task When(ProductRenamedEvent @event, StoreOrderDbContext context)
        {
            ProductInfoEntity product =
                await context.GetProjectionDocumentAsync<ProductInfoEntity>(_serializer, @event.ProductId);
            if (product == null) return;

            product.Name = @event.NewName;
            context.UpdateProjectionDocument(_serializer, product);
        }
        
        private async Task When(ProductAvailableEvent @event, StoreOrderDbContext context)
        {
            ProductInfoEntity product =
                await context.GetProjectionDocumentAsync<ProductInfoEntity>(_serializer, @event.ProductId);
            if (product == null) return;
            
            product.Available = true;
            
            context.UpdateProjectionDocument(_serializer, product);
        }
        
        private async Task When(ProductUnavailableEvent @event, StoreOrderDbContext context)
        {
            ProductInfoEntity product = 
                await context.GetProjectionDocumentAsync<ProductInfoEntity>(_serializer, @event.ProductId);
            if (product == null) return;

            product.Available = false;
            
            context.UpdateProjectionDocument(_serializer, product);
        }
    }
}