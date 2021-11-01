using System;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure.EntityFramework.Entity;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations
{
    public class ProductCreatedOperation : IProjectionOperation<ProductDisplayEntity>
    {
        private readonly ProductCreatedEvent _event;
        
        public ProductCreatedOperation(ProductCreatedEvent @event)
        {
            _event = @event ?? throw new ArgumentNullException(nameof(@event));
        }
            
        public ProductDisplayEntity Apply(ProductDisplayEntity productDisplayEntity)
        {
            productDisplayEntity.Id = productDisplayEntity.Id;
            productDisplayEntity.Name = _event.Name;
            productDisplayEntity.Description = _event.Description;
            productDisplayEntity.Price = _event.Price;

            return productDisplayEntity;
        }
    }
}