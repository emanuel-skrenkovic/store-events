using System;
using System.Threading.Tasks;
using Store.Catalogue.Integration.Events;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure.EntityFramework.Extensions;
using Store.Order.Infrastructure;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Product.Projections
{
    public class ProductInfoProjection : IProjection<ProductInfoEntity, StoreOrderDbContext>
    {
        private readonly ISerializer _serializer;

        public ProductInfoProjection(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        
        public Func<Task> Project(IEvent receivedEvent, StoreOrderDbContext context)
        {
            return receivedEvent switch
            {
                ProductAddedEvent @event        => () => When(@event, context),
                ProductPriceChangedEvent @event => () => When(@event, context),
                ProductRenamedEvent @event      => () => When(@event, context),
                ProductAvailableEvent @event    => () => When(@event, context),
                ProductUnavailableEvent @event  => () => When(@event, context),
                _ => null
            };
        }
        
        private Task When(ProductAddedEvent @event, StoreOrderDbContext context)
        {
            ProductInfoEntity product = new()
            {
                Id = @event.ProductId,
                Name = @event.Name,
                Price = @event.Price,
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