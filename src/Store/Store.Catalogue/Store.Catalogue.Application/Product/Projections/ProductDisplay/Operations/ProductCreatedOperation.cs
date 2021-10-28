using System;
using Store.Catalogue.Domain.Product.Events;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations
{
    public class ProductCreatedOperation : IProjectionOperation<ProductDisplay>
    {
        private readonly ProductCreatedEvent _event;
        
        public ProductCreatedOperation(ProductCreatedEvent @event)
        {
            _event = @event ?? throw new ArgumentNullException(nameof(@event));
        }
            
        public ProductDisplay Apply(ProductDisplay productDisplay)
        {
            return productDisplay with
            {
                Id = Guid.Empty, Name = _event.Name, Description = _event.Description, Price = _event.Price
            };
        }
    }
}