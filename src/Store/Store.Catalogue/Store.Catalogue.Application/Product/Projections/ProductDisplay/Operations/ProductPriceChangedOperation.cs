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
            productDisplay.Price = _event.NewPrice;
            return productDisplay;
        }
    }
}