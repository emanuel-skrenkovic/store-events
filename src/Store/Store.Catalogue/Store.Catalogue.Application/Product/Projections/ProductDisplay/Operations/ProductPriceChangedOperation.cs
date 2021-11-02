using System;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure.EntityFramework.Entity;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations
{
    public class ProductPriceChangedOperation : IProjectionOperation<ProductDisplayEntity>
    {
        private readonly ProductPriceChangedEvent _event;
        
        public ProductPriceChangedOperation(ProductPriceChangedEvent @event)
        {
            _event = @event ?? throw new ArgumentNullException(nameof(@event));
        }
        
        public ProductDisplayEntity Apply(ProductDisplayEntity productDisplayEntity)
        {
            productDisplayEntity.Price = _event.NewPrice;
            return productDisplayEntity;
        }
    }
}