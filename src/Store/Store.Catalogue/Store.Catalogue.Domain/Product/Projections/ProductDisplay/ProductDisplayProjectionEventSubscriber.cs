using System;
using System.Linq;
using System.Threading.Tasks;
using Store.Catalogue.Domain.Product.Events;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Domain.Product.Projections.ProductDisplay
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
        private readonly IProjection<ProductDisplay> _projection;
        
        public ProductDisplayProjectionEventSubscriber(IProjectionRunner runner, IProjection<ProductDisplay> projection)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
            _projection = projection ?? throw new ArgumentNullException(nameof(projection));
        }
        
        public Task Handle(IEvent @event)
        {
            if (!Handles(@event.GetType())) return Task.CompletedTask;
            
            return _runner.RunAsync(_projection, @event);
        }

        public bool Handles(Type type) => _supportedTypes.Contains(type);
    }
}