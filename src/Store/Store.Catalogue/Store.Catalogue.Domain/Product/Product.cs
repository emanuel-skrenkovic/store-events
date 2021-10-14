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

        public ICollection<short> Ratings { get; private set; }

        public double Rating => Ratings.Average(r => r);

        public ICollection<Tag> Tags { get; private set; }

        public static Product Create(string name, decimal price, string description = null)
        {
            Product product = new();
            product.ApplyEvent(new CreateProductEvent(name, price, description));

            return product;
        }
        
        private void Apply(CreateProductEvent domainEvent)
        {
            Id = Guid.NewGuid();
            
            Name = domainEvent.Name;
            Price = domainEvent.Price;
            Description = domainEvent.Description;
        }

        public void AddRating(AddRatingEvent e)
        {
            ApplyEvent(e);
        }

        private void Apply(AddRatingEvent domainEvent)
        {
            Ratings ??= new List<short>();
            Ratings.Add(domainEvent.Rating);
        }

        public void Tag(AddTagEvent domainEvent)
        {
            if (Tags.Any(t => t.Value == domainEvent.Tag.Value))
            {
                return;
            }
            
            ApplyEvent(domainEvent);
        }

        private void Apply(AddTagEvent domainEvent)
        {
            Tags ??= new List<Tag>();
            Tags.Add(domainEvent.Tag);
        }

        protected override void RegisterAppliers()
        {
            RegisterApplier<CreateProductEvent>(Apply);
            RegisterApplier<AddRatingEvent>(Apply);
            RegisterApplier<AddTagEvent>(Apply);
        }
    }
}