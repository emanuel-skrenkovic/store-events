using System;
using System.Threading.Tasks;
using Store.Catalogue.Application.Product.Projections.ProductDisplay.Operations;
using Store.Catalogue.Domain.Product.Events;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public class ProductDisplayProjection : IProjection
    {
        private readonly IProjectionRunner _runner;

        public ProductDisplayProjection(IProjectionRunner runner)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }
        
        public Task ProjectAsync(object receivedEvent) => 
            receivedEvent switch
            {
                ProductCreatedEvent productCreatedEvent           => _runner.RunAsync(new ProductCreatedOperation(productCreatedEvent)),
                ProductPriceChangedEvent productPriceChangedEvent => _runner.RunUpdateAsync(new ProductPriceChangedOperation(productPriceChangedEvent)),
                _                                                 => Task.CompletedTask
            };
    }
}