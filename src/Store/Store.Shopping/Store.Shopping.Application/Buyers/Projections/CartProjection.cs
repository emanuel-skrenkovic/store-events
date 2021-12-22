using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EntityFramework.Extensions;
using Store.Shopping.Domain.Buyers.Events;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Store.Shopping.Infrastructure;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Buyers.Projections;

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

        CartEntity cartEntity = await context.Carts.SingleOrDefaultAsync(
            c => c.CustomerNumber == buyerId.CustomerNumber &&
                 c.SessionId == buyerId.SessionId);
            
        string productCatalogueNumber = @event.ProductCatalogueNumber;
            
        if (cartEntity == null)
        {
            cartEntity = new()
            {
                CustomerNumber = buyerId.CustomerNumber,
                SessionId = buyerId.SessionId,
            };

            ProductEntity product = await context.FindAsync<ProductEntity>(productCatalogueNumber);
            if (product == null) throw new InvalidOperationException($"Product {productCatalogueNumber} does not exist.");

            Cart cart = new()
            {
                Entries = new() { 
                    [productCatalogueNumber] = new CartEntry
                    { 
                        Price = product.Price, 
                        Quantity = 1,
                        ProductInfo = new ProductInfo
                        {
                            CatalogueNumber = productCatalogueNumber,
                            Name = product.Name,
                            Price = product.Price
                        }
                    }
                }
            };

            cart.Price = cart.Entries.Values.Select(e => e.Price).Sum();

            cartEntity.Data = _serializer.Serialize(cart);
            context.Add(cartEntity);
        }
        else
        {
            Cart cart = _serializer.Deserialize<Cart>(cartEntity.Data);

            if (cart.Entries?.ContainsKey(productCatalogueNumber) == true)
            {
                CartEntry cartEntry = cart.Entries[productCatalogueNumber];
                    
                cartEntry.Quantity++;
                cartEntry.Price += cartEntry.ProductInfo.Price;
            }
            else
            {
                ProductEntity product = await context.FindAsync<ProductEntity>(productCatalogueNumber);
                if (product == null) throw new InvalidOperationException($"Product {productCatalogueNumber} does not exist.");
                    
                // TODO: be careful.
                cart.Entries![productCatalogueNumber] = new CartEntry
                {
                    Price = product.Price,
                    Quantity = 1,
                    ProductInfo = new ProductInfo
                    {
                        CatalogueNumber = productCatalogueNumber,
                        Name = product.Name,
                        Price = product.Price
                    }
                };
            }
                
            cart.Price = cart.Entries.Values.Select(e => e.Price).Sum();

            cartEntity.Data = _serializer.Serialize(cart);
            context.Update(cartEntity);
        }
    }

    private async Task When(BuyerCartItemRemovedEvent @event, StoreOrderDbContext context)
    {
        BuyerIdentifier buyerId = BuyerIdentifier.FromString(@event.BuyerId);

        CartEntity cartEntity = await context.Carts.SingleOrDefaultAsync(
            c => c.CustomerNumber == buyerId.CustomerNumber
                 && c.SessionId == buyerId.SessionId);
            
        if (cartEntity == null) return;

        Cart cart = _serializer.Deserialize<Cart>(cartEntity.Data);

        // TODO: ugly, ugly, ugly
        string productCatalogueNumber = @event.ProductCatalogueNumber;
        if (cart.Entries.TryGetValue(productCatalogueNumber, out CartEntry cartEntry))
        {
            if (--cartEntry.Quantity < 1)
            {
                cart.Entries.Remove(productCatalogueNumber);
            }
            else
            {
                cartEntry.Price -= cartEntry.ProductInfo.Price;
            }
        }
            
        cart.Price = cart.Entries.Values.Select(e => e.Price).Sum();

        cartEntity.Data = _serializer.Serialize(cart);
        context.Update(cartEntity);
    }
}