using System;
using System.Collections.Generic;
using System.Linq;
using Store.Catalogue.Domain.Product.Events;
using Store.Core.Domain;

namespace Store.Catalogue.Domain.Product
{
    public class Product : AggregateEntity<Guid>
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public decimal Price { get; private set; }
        
        public bool Available { get; private set; }

        public ICollection<ProductRating> Ratings { get; private set; }

        public ICollection<Tag> Tags { get; private set; }

        public static Product Create(Guid id, string name, decimal price, string description = null)
        {
            Product product = new();
            product.ApplyEvent(new ProductCreatedEvent(id, name, price, description)); // TODO: id

            return product;
        }
        
        private void Apply(ProductCreatedEvent domainCreatedEvent)
        {
            Id = domainCreatedEvent.EntityId;
            Name = domainCreatedEvent.Name;
            Price = domainCreatedEvent.Price;
            Description = domainCreatedEvent.Description;
        }

        public void Rename(string newName)
        {
            if (Name == newName) return;

            ApplyEvent(new ProductRenamedEvent(Id, newName));
        }
        
        private void Apply(ProductRenamedEvent domainEvent)
        {
            Name = domainEvent.NewName;
        }

        public void ChangePrice(decimal newPrice, string reason = null)
        {
            ApplyEvent(new ProductPriceChangedEvent(Id, newPrice, reason));
        }

        private void Apply(ProductPriceChangedEvent domainEvent)
        {
            Price = domainEvent.NewPrice;
        }

        public void MarkAvailable()
        {
            if (Available) return;
            ApplyEvent(new ProductMarkedAvailableEvent(Id));
        }

        public void Apply(ProductMarkedAvailableEvent domainEvent)
        {
            Available = true;
        }

        public void MarkUnavailable()
        {
            if (!Available) return;
            ApplyEvent(new ProductMarkedUnavailableEvent(Id));
        }
        
        public void Apply(ProductMarkedUnavailableEvent domainEvent)
        {
            Available = false;
        }

        public void AddRating(ProductRating productRating)
        {
            ApplyEvent(new ProductRatedEvent(Id, productRating));
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
            
            ApplyEvent(new ProductTaggedEvent(Id, tag));
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
            RegisterApplier<ProductRenamedEvent>(Apply);
            RegisterApplier<ProductMarkedAvailableEvent>(Apply);
            RegisterApplier<ProductMarkedUnavailableEvent>(Apply);
        }
    }
}