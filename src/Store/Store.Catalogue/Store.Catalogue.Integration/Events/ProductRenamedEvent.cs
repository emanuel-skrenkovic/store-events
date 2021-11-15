using System;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Integration.Events
{
    public record ProductRenamedEvent(Guid ProductId, string NewName) : IIntegrationEvent;
}