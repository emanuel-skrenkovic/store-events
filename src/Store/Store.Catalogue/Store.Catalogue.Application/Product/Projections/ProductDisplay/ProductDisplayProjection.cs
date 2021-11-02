using Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure.EntityFramework.Entity;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public class ProductDisplayProjection : IProjection<ProductDisplayEntity>
    {
        public ProductDisplayEntity Project(ProductDisplayEntity productDisplayEntity, object receivedEvent) => 
            receivedEvent switch
            {
                ProductCreatedEvent productCreatedEvent           => new ProductCreatedOperation(productCreatedEvent).Apply(productDisplayEntity),
                ProductPriceChangedEvent productPriceChangedEvent => new ProductPriceChangedOperation(productPriceChangedEvent).Apply(productDisplayEntity),
                _                                                 => productDisplayEntity
            };
    }
}