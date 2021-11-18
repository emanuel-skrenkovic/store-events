using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure.EntityFramework.Extensions;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public class ProductDisplayProjection : IEventListener, IProjection
    {
        private const string SubscriptionId = nameof(ProductDisplayEntity);
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISerializer _serializer;
        private readonly IEventSubscriptionFactory _eventSubscriptionFactory;

        public ProductDisplayProjection(
            IServiceScopeFactory scopeFactory,
            ISerializer serializer,
            IEventSubscriptionFactory eventSubscriptionFactory)
        {
            _scopeFactory             = scopeFactory             ?? throw new ArgumentNullException(nameof(scopeFactory));
            _serializer               = serializer               ?? throw new ArgumentNullException(nameof(serializer));
            _eventSubscriptionFactory = eventSubscriptionFactory ?? throw new ArgumentNullException(nameof(eventSubscriptionFactory));
        }
        
        #region EventListener
        
        public async Task StartAsync()
        {
            using IServiceScope scope = _scopeFactory.CreateScope();

            StoreCatalogueDbContext context = scope.ServiceProvider.GetRequiredService<StoreCatalogueDbContext>();

            if (context == null)
            {
                throw new InvalidOperationException($"Context cannot be null on {nameof(ProductDisplayProjection)} startup.");
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

        #endregion
        
        #region Projection

        public async Task ProjectAsync(IEvent receivedEvent, EventMetadata eventMetadata)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();

            StoreCatalogueDbContext context = scope.ServiceProvider.GetRequiredService<StoreCatalogueDbContext>();
            if (context == null) return; // TODO: error or log?
                
            Func<Task> projectionAction = receivedEvent switch
            {
                ProductCreatedEvent @event           => () => When(@event, context),
                ProductPriceChangedEvent @event      => () => When(@event, context),
                ProductRenamedEvent @event           => () => When(@event, context),
                ProductMarkedAvailableEvent @event   => () => When(@event, context),
                ProductMarkedUnavailableEvent @event => () => When(@event, context),
                _ => null
            };

            if (projectionAction == null) return;

            await projectionAction();
            await context.AddOrUpdateSubscriptionCheckpoint(SubscriptionId, eventMetadata.StreamPosition);
            
            await context.SaveChangesAsync();
        }
        
        private Task When(ProductCreatedEvent @event, StoreCatalogueDbContext context)
        {
            ProductDisplayEntity productDisplay = new()
            {
                Id = @event.EntityId,
                Name = @event.Name,
                Price = @event.Price,
                Description = @event.Description
            };
            
            context.AddProjectionDocument(_serializer, productDisplay);

            return Task.CompletedTask;
        }
        
        private async Task When(ProductPriceChangedEvent @event, StoreCatalogueDbContext context)
        {
            ProductDisplayEntity productDisplay = 
                await context.GetProjectionDocumentAsync<ProductDisplayEntity>(_serializer, @event.EntityId);
            if (productDisplay == null) return;

            productDisplay.Price = @event.NewPrice;
            context.UpdateProjectionDocument(_serializer, productDisplay);
        }
        
        private async Task When(ProductRenamedEvent @event, StoreCatalogueDbContext context)
        {
            ProductDisplayEntity productDisplay = 
                await context.GetProjectionDocumentAsync<ProductDisplayEntity>(_serializer, @event.EntityId);
            if (productDisplay == null) return;

            productDisplay.Name = @event.NewName;
            context.UpdateProjectionDocument(_serializer, productDisplay);
        }
        
        private async Task When(ProductMarkedAvailableEvent @event, StoreCatalogueDbContext context)
        {
            ProductDisplayEntity productDisplay = 
                await context.GetProjectionDocumentAsync<ProductDisplayEntity>(_serializer, @event.EntityId);
            if (productDisplay == null) return;
            
            productDisplay.Available = true;
            
            context.UpdateProjectionDocument(_serializer, productDisplay);
        }
        
        private async Task When(ProductMarkedUnavailableEvent @event, StoreCatalogueDbContext context)
        {
            ProductDisplayEntity productDisplay = 
                await context.GetProjectionDocumentAsync<ProductDisplayEntity>(_serializer, @event.EntityId);
            if (productDisplay == null) return;

            productDisplay.Available = false;
            
            context.UpdateProjectionDocument(_serializer, productDisplay);
        }
        
        #endregion
    }
}