using System;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure.EventStore;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public class ProductDisplayProjectionManager : EventStoreProjectionManager
    {
        private readonly IProjectionRunner<ProductDisplayEntity> _runner;
        private readonly IProjection<ProductDisplayEntity> _projection;
        
        // TODO: WTF is this?
        public ProductDisplayProjectionManager(
            IProjectionRunner<ProductDisplayEntity> runner, 
            IProjection<ProductDisplayEntity> projection,
            
            EventStoreSubscriptionConfiguration configuration,
            EventStoreClient eventStore, 
            ICheckpointRepository checkpointRepository,
            ISerializer serializer) : base(
                configuration with { SubscriptionId = typeof(ProductDisplayEntity).AssemblyQualifiedName },
                eventStore,
                checkpointRepository,
                serializer)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
            _projection = projection ?? throw new ArgumentNullException(nameof(projection));
        }

        protected override Task ProjectEventAsync(IEvent @event)
        {
            return _runner.RunAsync(_projection, @event);
        }
    }
}