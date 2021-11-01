using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Domain.Product.Projections.ProductDisplay.Operations;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Domain.Product.Projections.ProductDisplay
{
    public class ProductDisplayProjection : IProjection<ProductDisplay>
    {
        public ProductDisplay Project(ProductDisplay productDisplay, object receivedEvent) => 
            receivedEvent switch
            {
                ProductCreatedEvent productCreatedEvent           => new ProductCreatedOperation(productCreatedEvent).Apply(productDisplay),
                ProductPriceChangedEvent productPriceChangedEvent => new ProductPriceChangedOperation(productPriceChangedEvent).Apply(productDisplay),
                _                                                 => productDisplay
            };
    }
}