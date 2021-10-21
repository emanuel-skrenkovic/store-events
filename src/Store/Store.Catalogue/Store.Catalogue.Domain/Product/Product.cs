using System;
using System.Collections.Generic;
using System.Linq;
using Store.Catalogue.Domain.Product.Events;
using Store.Core.Domain;

namespace Store.Catalogue.Domain.Product
{
    public class Product : AggregateEntity
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public decimal Price { get; private set; }

        public ICollection<ProductRating> Ratings { get; private set; }

        public ICollection<Tag> Tags { get; private set; }

        public static Product Create(string name, decimal price, string description = null)
        {
            Product product = new();
            product.ApplyEvent(new ProductCreatedEvent(name, price, description));

            return product;
        }
        
        private void Apply(ProductCreatedEvent domainCreatedEvent)
        {
            Id = Guid.NewGuid();
            
            Name = domainCreatedEvent.Name;
            Price = domainCreatedEvent.Price;
            Description = domainCreatedEvent.Description;
        }

        public void ChangePrice(decimal newPrice, string reason = null)
        {
            ApplyEvent(new ProductPriceChangedEvent(newPrice, reason));
        }

        private void Apply(ProductPriceChangedEvent domainEvent)
        {
            Price = domainEvent.NewPrice;
        }

        public void AddRating(ProductRating productRating)
        {
            ApplyEvent(new ProductRatedEvent(productRating));
        }

        private void Apply(ProductRatedEvent domainRatedEvent)
        {
            Ratings ??= new List<ProductRating>();
            Ratings.Add(domainRatedEvent.ProductRating);
        }

        public void Tag(Tag tag)
        {
            if (Tags?.Any(t => t.Value == tag.Value) == true)
            {
                return;
            }
            
            ApplyEvent(new ProductTaggedEvent(tag));
        }

        private void Apply(ProductTaggedEvent domainEvent)
        {
            Tags ??= new List<Tag>();
            Tags.Add(domainEvent.Tag);
        }

        protected override void RegisterAppliers()
        {
            RegisterApplier<ProductCreatedEvent>(Apply);
            RegisterApplier<ProductRatedEvent>(Apply);
            RegisterApplier<ProductTaggedEvent>(Apply);
            RegisterApplier<ProductPriceChangedEvent>(Apply);
        }
    }
}