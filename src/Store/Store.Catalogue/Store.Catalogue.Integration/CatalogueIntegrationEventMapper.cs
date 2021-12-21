using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Integration.Events;
using Store.Core.Domain.Event;
using ProductPriceChangedEvent = Store.Catalogue.Domain.Product.Events.ProductPriceChangedEvent;
using ProductRenamedEvent = Store.Catalogue.Domain.Product.Events.ProductRenamedEvent;

namespace Store.Catalogue.Integration;

// TODO: think about how to model the integration events.
public class CatalogueIntegrationEventMapper : IIntegrationEventMapper
{
    public bool TryMap(IEvent domainEvent, out IEvent integrationEvent)
    {
        integrationEvent = domainEvent switch
        {
            ProductCreatedEvent @event           => Map(@event),
            ProductPriceChangedEvent @event      => Map(@event),
            ProductRenamedEvent @event           => Map(@event),
            ProductMarkedAvailableEvent @event   => Map(@event),
            ProductMarkedUnavailableEvent @event => Map(@event),
            _ => null
        };

        return integrationEvent != null;
    }

    private ProductAddedEvent Map(ProductCreatedEvent domainEvent)
        => new(domainEvent.EntityId, domainEvent.Name, domainEvent.Description, domainEvent.Price);

    private Integration.Events.ProductPriceChangedEvent Map(ProductPriceChangedEvent domainEvent)
        => new(domainEvent.EntityId, domainEvent.NewPrice);

    private Integration.Events.ProductRenamedEvent Map(ProductRenamedEvent domainEvent)
        => new(domainEvent.EntityId, domainEvent.NewName);

    private ProductAvailableEvent Map(ProductMarkedAvailableEvent domainEvent)
        => new(domainEvent.EntityId);

    private ProductUnavailableEvent Map(ProductMarkedUnavailableEvent domainEvent)
        => new(domainEvent.EntityId);
}