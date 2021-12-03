using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EntityFramework.Extensions;
using Store.Order.Domain.Buyers.Events;
using Store.Order.Domain.Buyers.ValueObjects;
using Store.Order.Infrastructure;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Buyer.Projections.Cart
{
    public class CartProjection : IEventListener
    {
        private const string SubscriptionId = nameof(CartEntryEntity);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEventSubscriptionFactory _eventSubscriptionFactory;

        public CartProjection(
            IServiceScopeFactory scopeFactory,
            IEventSubscriptionFactory eventSubscriptionFactory)
        {
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
        
        private async Task HandleEventAsync(IEvent receivedEvent, EventMetadata eventMetadata)
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
            BuyerIdentifier buyerId = BuyerIdentifier.FromString(@event.BuyerId);

            CartEntryEntity cartEntry = await context.CartEntries.SingleOrDefaultAsync(
                c => c.CustomerNumber == buyerId.CustomerNumber
                     && c.SessionId == buyerId.SessionId
                     && c.ProductCatalogueNumber == @event.ProductCatalogueNumber);
            
            if (cartEntry == null)
            {
                cartEntry = new()
                {
                    CustomerNumber = buyerId.CustomerNumber,
                    SessionId = buyerId.SessionId,
                    ProductCatalogueNumber = @event.ProductCatalogueNumber,
                    Quantity = 1
                };
                
                context.Add(cartEntry);
            }
            else
            {
                cartEntry.Quantity++;
                
                context.Update(cartEntry);
            }
        }

        private async Task When(BuyerCartItemRemovedEvent @event, StoreOrderDbContext context)
        {
            BuyerIdentifier buyerId = BuyerIdentifier.FromString(@event.BuyerId);
            
            CartEntryEntity cartEntry = await context.CartEntries.SingleOrDefaultAsync(
                c => c.CustomerNumber == buyerId.CustomerNumber
                     && c.SessionId == buyerId.SessionId
                     && c.ProductCatalogueNumber == @event.ProductCatalogueNumber);
            
            if (cartEntry == null) return;

            if (--cartEntry.Quantity < 1) context.Remove(cartEntry);
            else                          context.Update(cartEntry);
        }
    }
}