using System;
using System.Collections.Generic;
using Store.Cart.Domain.Events;
using Store.Core.Domain;

namespace Store.Cart.Domain
{
    public class Cart : AggregateEntity
    {
        public Guid CustomerId { get; private set; }
        
        public List<CartItem> Items { get; private set; }

        public static Cart Create(Guid id, Guid customerId)
        {
            Cart cart = new();
            cart.ApplyEvent(new CartCreatedEvent(id, customerId));

            return cart;
        }

        private void Apply(CartCreatedEvent domainEvent)
        {
            Id = domainEvent.EntityId;
            CustomerId = domainEvent.CustomerId;
        }

        public void AddItem(CartItem item)
        {
            ApplyEvent(new CartItemAdded(Id, item));
        }
        
        public void Apply(CartItemAdded domainEvent)
        {
            Items ??= new List<CartItem>();
            Items.Add(domainEvent.Item);
        }
        
        protected override void RegisterAppliers()
        {
            RegisterApplier<CartItemAdded>(Apply);
        }
    }
}