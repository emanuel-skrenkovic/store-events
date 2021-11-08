using System;
using System.Collections.Generic;
using Store.Core.Domain;
using Store.Order.Domain.Buyers.Events;

namespace Store.Order.Domain.Buyers
{
    public class Buyer : AggregateEntity
    { 
        public CustomerNumber CustomerNumber { get; private set; }
        
        public Cart Cart { get; private set; }
        
        private Buyer() { }
       
        public static Buyer Create(Guid id, CustomerNumber customerNumber)
        {
            Buyer buyer = new();
            buyer.ApplyEvent(new BuyerCreatedEvent(id, customerNumber));

            return buyer;
        }

        private void Apply(BuyerCreatedEvent domainEvent)
        {
            Id = domainEvent.EntityId;
            CustomerNumber = domainEvent.CustomerNumber;
            Cart = new Cart(new Dictionary<Item, uint>()); // TODO: cleaner
        }

        public void AddCartItem(Item item)
        {
            ApplyEvent(new BuyerCartItemAddedEvent(Id, item));
        }

        private void Apply(BuyerCartItemAddedEvent domainEvent)
        {
            Dictionary<Item, uint> cartItems = Cart.Items;

            Item item = domainEvent.Item;
            
            if (cartItems.ContainsKey(item))
            {
                cartItems[item]++;
            }
            else
            {
                Cart.Items.Add(item, 1);
            }            
        }

        public void RemoveCartItem(Item item)
        {
            if (!Cart.Items.ContainsKey(item)) return;
            ApplyEvent(new BuyerCartItemRemovedEvent(Id, item));
        }

        private void Apply(BuyerCartItemRemovedEvent domainEvent)
        {
            Dictionary<Item, uint> cartItems = Cart.Items;

            Item item = domainEvent.Item;

            if (!cartItems.ContainsKey(item))
                return;
            
            if (--cartItems[item] == 0)
            {
                cartItems.Remove(item);
            }
        }
       
        protected override void RegisterAppliers()
        {
            RegisterApplier<BuyerCreatedEvent>(Apply);
            RegisterApplier<BuyerCartItemAddedEvent>(Apply);
            RegisterApplier<BuyerCartItemRemovedEvent>(Apply);
        }
    }
}