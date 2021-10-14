using Store.Core.Domain;

namespace Store.Catalogue.Domain.Product.Events
{
    public record AddTagEvent : IEvent
    {
        public Tag Tag { get; }
    }
}