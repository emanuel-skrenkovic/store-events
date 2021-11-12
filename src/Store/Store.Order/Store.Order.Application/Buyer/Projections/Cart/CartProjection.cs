using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure.EntityFramework.Extensions;
using Store.Order.Domain.Buyers.Events;
using Store.Order.Infrastructure;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Buyer.Projections.Cart
{
    public class CartProjection : IProjection<CartEntity, StoreOrderDbContext>
    {
        private readonly ISerializer _serializer;

        public CartProjection(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        
        public Func<Task> Project(IEvent receivedEvent, StoreOrderDbContext context)
        {
            return receivedEvent switch
            {
                BuyerCartItemAddedEvent   @event => () => When(@event, context),
                BuyerCartItemRemovedEvent @event => () => When(@event, context),
                // TODO: order confirmed -> remove cart
                _ => null
            };
        }

        private async Task When(BuyerCartItemAddedEvent @event, StoreOrderDbContext context)
        {
            CartEntity cart = await context.GetProjectionDocumentByAsync<CartEntity>(
                _serializer, 
                c => c.CustomerNumber == @event.EntityId);

            bool notFound = cart == null;
            if (notFound)
            {
                cart = new()
                {
                    Id = Guid.NewGuid(), 
                    CustomerNumber = @event.EntityId
                };
            }

            cart.Items ??= new Dictionary<string, CartEntryEntity>();

            Dictionary<string, CartEntryEntity> items = cart.Items;
            string newItemCatalogueNumber = @event.Item.CatalogueNumber.Value;
            
            if (items.ContainsKey(newItemCatalogueNumber))
            {
                items[newItemCatalogueNumber].Quantity++;
            }
            else
            {
                cart.Items.Add(newItemCatalogueNumber, new CartEntryEntity
                {
                    CatalogueNumber = @event.Item.CatalogueNumber.Value,
                    Quantity = 1
                });
            }
            
            if (notFound)
            {
                context.AddProjectionDocument(_serializer, cart);
            }
            else
            {
                context.UpdateProjectionDocument(_serializer, cart);
            }
        }

        private async Task When(BuyerCartItemRemovedEvent @event, StoreOrderDbContext context)
        {
            CartEntity cart = await context.GetProjectionDocumentByAsync<CartEntity>(_serializer, c => c.CustomerNumber == @event.EntityId);

            if (cart?.Items.Any() != true)
            {
                return;
            }

            var cartItems = cart.Items;
            string itemCatalogueNumber = @event.Item.CatalogueNumber.Value;
            if (!cart.Items.ContainsKey(itemCatalogueNumber))
                return;
            
            if (--cartItems[itemCatalogueNumber].Quantity == 0)
            {
                cartItems.Remove(itemCatalogueNumber);
            }

            context.UpdateProjectionDocument(_serializer, cart);
        }
    }
}