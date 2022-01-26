using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Store.Catalogue.Infrastructure.Entity;
using Store.Catalogue.Integration;
using Store.Catalogue.Integration.Events;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Infrastructure.Integration;

public class CatalogueIntegrationEventMapper : IIntegrationEventMapper
{
    public bool TryMap(object change, out IEvent integrationEvent)
    {
        EntityEntry entry = (EntityEntry) change;
        EntityState entityState = entry.State;

        integrationEvent = null;

        if (entry.Entity is ProductEntity product)
        {
            ProductView productView = new(product.Name, product.Price, product.Available, product.Description);
            integrationEvent = entityState switch
            {
                EntityState.Added    => new ProductCreatedEvent(product.CatalogueId, productView),
                EntityState.Modified => new ProductUpdatedEvent(product.CatalogueId, productView),
                _ => null
            };
        }

        return integrationEvent != null;
    }
}