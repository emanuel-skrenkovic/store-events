using Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public class ProductDisplayProjection : IProjection<ProductDisplayEntity>
    {
        public ProductDisplayEntity Project(ProductDisplayEntity productDisplayEntity, object receivedEvent) => 
            receivedEvent switch
            {
                ProductCreatedEvent @event           => new ProductCreatedOperation(@event).Apply(productDisplayEntity),
                ProductPriceChangedEvent @event      => new ProductPriceChangedOperation(@event).Apply(productDisplayEntity),
                ProductRenamedEvent @event           => new ProductRenamedOperation(@event).Apply(productDisplayEntity),
                ProductMarkedAvailableEvent @event   => new ProductMarkedAvailableOperation(@event).Apply(productDisplayEntity),
                ProductMarkedUnavailableEvent @event => new ProductMarkedUnavailableOperation(@event).Apply(productDisplayEntity),
                _                                                 => productDisplayEntity
            };
    }
}