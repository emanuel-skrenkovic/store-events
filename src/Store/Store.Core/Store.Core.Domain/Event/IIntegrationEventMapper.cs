namespace Store.Core.Domain.Event;

public interface IIntegrationEventMapper
{
    bool TryMap(object change, out IEvent integrationEvent);
}