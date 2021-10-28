using System;
using Store.Catalogue.Domain.Product.Events;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations
{
    public class ProductPriceChangedOperation : IProjectionOperation<ProductDisplay>
    {
        private readonly ProductPriceChangedEvent _event;
        
        public ProductPriceChangedOperation(ProductPriceChangedEvent @event)
        {
            _event = @event ?? throw new ArgumentNullException(nameof(@event));
        }
        
        public ProductDisplay Apply(ProductDisplay productDisplay)
        {
            // TODO: think about using records. Mutations probably work better.
            return productDisplay with { Price = _event.NewPrice };
        }
    }
}