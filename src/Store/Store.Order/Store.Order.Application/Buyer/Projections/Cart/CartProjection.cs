using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EntityFramework.Extensions;
using Store.Order.Domain.Buyers.Events;
using Store.Order.Infrastructure;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Buyer.Projections.Cart
{
    public class CartProjection : IEventListener
    {
        private const string SubscriptionId = nameof(CartEntity);

        private readonly ISerializer _serializer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEventSubscriptionFactory _eventSubscriptionFactory;

        public CartProjection(
            ISerializer serializer,
            IServiceScopeFactory scopeFactory,
            IEventSubscriptionFactory eventSubscriptionFactory)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _eventSubscriptionFactory = eventSubscriptionFactory ?? throw new ArgumentNullException(nameof(eventSubscriptionFactory));
        }
        
        public async Task StartAsync()
        {
            using IServiceScope scope = _scopeFactory.CreateScope();

            StoreOrderDbContext context = scope.ServiceProvider.GetRequiredService<StoreOrderDbContext>();
            if (context == null)
            {
                throw new InvalidOperationException($"Context cannot be null on {nameof(CartProjection)} startup.");
            }
            
            ulong checkpoint = await context.GetSubscriptionCheckpoint(SubscriptionId);
            
            await _eventSubscriptionFactory
                .Create(SubscriptionId, HandleEventAsync)
                .SubscribeAtAsync(checkpoint);
        }

        public Task StopAsync() => Task.CompletedTask;
        
        public async Task HandleEventAsync(IEvent receivedEvent, EventMetadata eventMetadata)
        {
            Ensure.NotNull(receivedEvent, nameof(receivedEvent));

            using IServiceScope scope = _scopeFactory.CreateScope();

            StoreOrderDbContext context = scope.ServiceProvider.GetRequiredService<StoreOrderDbContext>();
            if (context == null) return;

            Func<Task> projectionAction = receivedEvent switch
            {
                BuyerCartItemAddedEvent   @event => () => When(@event, context),
                BuyerCartItemRemovedEvent @event => () => When(@event, context),
                // TODO: order confirmed -> remove cart
                _ => null
            };
            if (projectionAction == null) return;

            await projectionAction();
            await context.AddOrUpdateSubscriptionCheckpoint(SubscriptionId, eventMetadata.StreamPosition);
            
            await context.SaveChangesAsync();
        }
        
        private async Task When(BuyerCartItemAddedEvent @event, StoreOrderDbContext context)
        {
            CartEntity cart = await context.GetProjectionDocumentByAsync<CartEntity>(
                _serializer, 
                c => c.CustomerNumber == @event.BuyerId);

            bool notFound = cart == null;
            if (notFound)
            {
                cart = new()
                {
                    Id = Guid.NewGuid(), 
                    CustomerNumber = @event.BuyerId
                };
            }

            cart.Items ??= new Dictionary<string, CartEntryEntity>();

            Dictionary<string, CartEntryEntity> items = cart.Items;
            string newItemCatalogueNumber = @event.ItemCatalogueNumber;
            
            if (items.ContainsKey(newItemCatalogueNumber))
            {
                items[newItemCatalogueNumber].Quantity++;
            }
            else
            {
                cart.Items.Add(newItemCatalogueNumber, new CartEntryEntity
                {
                    CatalogueNumber = newItemCatalogueNumber,
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
            CartEntity cart = await context.GetProjectionDocumentByAsync<CartEntity>(_serializer, c => c.CustomerNumber == @event.BuyerId);

            if (cart?.Items.Any() != true)
            {
                return;
            }

            var cartItems = cart.Items;
            string itemCatalogueNumber = @event.ItemCatalogueNumber;
            
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