﻿using Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations;
using Store.Catalogue.Domain.Product.Events;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
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