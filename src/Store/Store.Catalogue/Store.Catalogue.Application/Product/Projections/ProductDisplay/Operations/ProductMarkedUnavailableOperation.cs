using System;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations
{
    public class ProductMarkedUnavailableOperation : IProjectionOperation<ProductDisplayEntity>
    {
        private readonly ProductMarkedUnavailableEvent _event;

        public ProductMarkedUnavailableOperation(ProductMarkedUnavailableEvent @event)
        {
            _event = @event ?? throw new ArgumentNullException(nameof(@event));
        }
        
        public ProductDisplayEntity Apply(ProductDisplayEntity model)
        {
            model.Available = false;
            return model;
        }
    }
}