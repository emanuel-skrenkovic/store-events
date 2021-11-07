using System;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations
{
    public class ProductRenamedOperation : IProjectionOperation<ProductDisplayEntity>
    {
        private readonly ProductRenamedEvent _event;

        public ProductRenamedOperation(ProductRenamedEvent @event)
        {
            _event = @event ?? throw new ArgumentNullException(nameof(@event));
        }
        
        public ProductDisplayEntity Apply(ProductDisplayEntity model)
        {
            model.Name = _event.NewName;
            return model;
        }
    }
}