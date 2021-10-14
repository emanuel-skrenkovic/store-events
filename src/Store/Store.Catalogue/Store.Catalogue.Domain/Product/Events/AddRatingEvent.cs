using Store.Core.Domain;

namespace Store.Catalogue.Domain.Product.Events
{
    public record AddRatingEvent : IEvent
    {
        public short Rating { get; }
    }
}