using System;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations
{
    public class ProductMarkedAvailableOperation : IProjectionOperation<ProductDisplayEntity>
    {
        private readonly IEvent _event;
        
        public ProductMarkedAvailableOperation(ProductMarkedAvailableEvent @event)
        {
            _event = @event ?? throw new ArgumentNullException(nameof(@event));
        }
        
        public ProductDisplayEntity Apply(ProductDisplayEntity model)
        {
            model.Available = true;
            return model;
        }
    }
}