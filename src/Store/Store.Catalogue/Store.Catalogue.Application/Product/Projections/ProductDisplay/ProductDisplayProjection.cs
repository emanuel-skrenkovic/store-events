using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure.EntityFramework.Extensions;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public class ProductDisplayProjection : IProjection<ProductDisplayEntity, StoreCatalogueDbContext>
    {
        private readonly ISerializer _serializer;

        public ProductDisplayProjection(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        
        public Func<Task> Project(IEvent receivedEvent, StoreCatalogueDbContext context)
        {
            return receivedEvent switch
            {
                ProductCreatedEvent @event           => () => When(@event, context),
                ProductPriceChangedEvent @event      => () => When(@event, context),
                ProductRenamedEvent @event           => () => When(@event, context),
                ProductMarkedAvailableEvent @event   => () => When(@event, context),
                ProductMarkedUnavailableEvent @event => () => When(@event, context),
                _ => null
            };
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
    }
}