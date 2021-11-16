using System;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Integration.Events
{
    public record ProductAddedEvent(
        Guid ProductId, 
        string Name, 
        string Description, 
        decimal Price) : IEvent;
}