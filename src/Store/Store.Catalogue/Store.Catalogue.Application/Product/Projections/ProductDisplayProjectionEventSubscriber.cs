using System;
using System.Linq;
using System.Threading.Tasks;
using Store.Catalogue.Domain.Product.Events;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections
{
    public class ProductDisplayProjectionEventSubscriber : IEventSubscriber
    {
        private readonly Type[] _supportedTypes =
        {
            typeof(ProductCreatedEvent),
            typeof(ProductPriceChangedEvent),
            typeof(ProductRatedEvent),
            typeof(ProductTaggedEvent)
        };
            
        private readonly IProjectionRunner _runner;
        private readonly IProjection<ProductDisplay.ProductDisplay> _projection;
        
        public ProductDisplayProjectionEventSubscriber(
            IProjectionRunner runner,
            IProjection<ProductDisplay.ProductDisplay> projection)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
            _projection = projection ?? throw new ArgumentNullException(nameof(projection));
        }
        
        public Task Handle(object @event)
        {
            return _runner.RunAsync(_projection, @event);
        }

        public bool Handles(Type type) => _supportedTypes.Contains(type);
    }
}