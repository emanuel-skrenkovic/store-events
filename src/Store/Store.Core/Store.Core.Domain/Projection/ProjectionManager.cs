using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Store.Core.Domain.Event;

namespace Store.Core.Domain.Projection
{
    public class ProjectionManager : IEventSubscriber // TODO: think about IProjectionManager interface
    {
        private readonly IReadOnlyCollection<IProjection> _projections;

        public ProjectionManager(IReadOnlyCollection<IProjection> projections)
        {
            _projections = projections ?? throw new ArgumentNullException(nameof(projections));
        }

        // TODO: remove from interface, probably.
        public bool Handles(string eventType) => true;

        public async Task Handle(object domainEvent)
        {
            // TODO: Think about async strategy here. Error handling might be important.
            foreach (IProjection projection in _projections)
            {
                try
                {
                    await projection.ProjectAsync(domainEvent);
                }
                catch
                {
                    // TODO: logging?
                }
            }
        }
    }
}