using System.Collections.Generic;
using Store.Core.Domain;
using Store.Order.Domain.Buyers.Events;
using Store.Order.Domain.Buyers.ValueObjects;

namespace Store.Order.Domain.Buyers
{
    public class Buyer : AggregateEntity<string>
    { 
        public string CustomerNumber { get; private set; }
        
        public string SessionId { get; private set; }
        
        public Dictionary<string, uint> CartItems { get; private set; }
        
        public static Buyer Create(BuyerIdentifier buyerIdentifier)
        {
            Buyer buyer = new();
            buyer.ApplyEvent(new BuyerCreatedEvent(
                buyerIdentifier.CustomerNumber, 
                buyerIdentifier.SessionId));

            return buyer;
        }

        private void Apply(BuyerCreatedEvent domainEvent)
        {
            Id = $"{domainEvent.CustomerNumber}-{domainEvent.SessionId}";
            CustomerNumber = domainEvent.CustomerNumber;
            SessionId = domainEvent.SessionId;
            CartItems = new();
        }

        public void AddCartItem(CatalogueNumber catalogueNumber) => ApplyEvent(new BuyerCartItemAddedEvent(Id, catalogueNumber.Value));

        private void Apply(BuyerCartItemAddedEvent domainEvent)
        {
            string catalogueNumber = domainEvent.ItemCatalogueNumber;
            
            if (CartItems.ContainsKey(catalogueNumber))
            {
                CartItems[catalogueNumber]++;
            }
            else
            {
                CartItems.Add(catalogueNumber, 1);
            }            
        }

        public void RemoveCartItem(CatalogueNumber catalogueNumber)
        {
            if (!CartItems.ContainsKey(catalogueNumber.Value)) return;
            ApplyEvent(new BuyerCartItemRemovedEvent(Id, catalogueNumber.Value));
        }

        private void Apply(BuyerCartItemRemovedEvent domainEvent)
        {
            string catalogueNumber = domainEvent.ItemCatalogueNumber;
            
            if (--CartItems[catalogueNumber] == 0)
            {
                CartItems.Remove(catalogueNumber);
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