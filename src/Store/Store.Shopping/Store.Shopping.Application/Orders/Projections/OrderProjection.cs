using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EntityFramework.Extensions;
using Store.Shopping.Application.Buyers.Projections;
using Store.Shopping.Domain.Orders.Events;
using Store.Shopping.Infrastructure;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Orders.Projections;

public class OrderProjection : IEventListener
{
    private const string SubscriptionId = nameof(OrderEntity);
    
    private readonly ISerializer _serializer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventSubscriptionFactory _eventSubscriptionFactory;

    public OrderProjection(
        ISerializer serializer,
        IServiceScopeFactory scopeFactory, 
        IEventSubscriptionFactory eventSubscriptionFactory)
    {
        _serializer               = Ensure.NotNull(serializer);
        _scopeFactory             = Ensure.NotNull(scopeFactory);
        _eventSubscriptionFactory = Ensure.NotNull(eventSubscriptionFactory);
    }

    public async Task StartAsync()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        StoreShoppingDbContext context = scope.ServiceProvider.GetRequiredService<StoreShoppingDbContext>();
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
        Ensure.NotNull(receivedEvent);
        Ensure.NotNull(eventMetadata);
        
        using IServiceScope scope = _scopeFactory.CreateScope();
        StoreShoppingDbContext context = scope.ServiceProvider.GetService<StoreShoppingDbContext>();
        if (context == null) return;
        
        Func<Task> projectionAction = receivedEvent switch
        {
            OrderCreatedEvent                @event => () => When(@event, context),
            OrderShippingInformationSetEvent @event => () => When(@event, context),
            OrderPaymentSubmittedEvent       @event => () => When(@event, context),
            OrderConfirmedEvent              @event => () => When(@event, context),
            _ => null
        };
        if (projectionAction == null) return;

        await projectionAction();
        await context.AddOrUpdateSubscriptionCheckpoint(SubscriptionId, eventMetadata.StreamPosition);

        await context.SaveChangesAsync();
    }

    private async Task When(OrderCreatedEvent @event, StoreShoppingDbContext context)
    {
        OrderEntity orderEntity = new()
        {
            OrderId = @event.OrderId,
            CustomerNumber = @event.CustomerNumber
        };

        DateTime now = DateTime.UtcNow;
        orderEntity.CreatedAt = now;
        orderEntity.UpdatedAt = now;

        IEnumerable<string>      productIds = @event.OrderLines.Select(ol => ol.CatalogueNumber);
        ICollection<ProductEntity> products = await context.Products.Where(p => productIds.Contains(p.CatalogueNumber)).ToListAsync();
        
        Order order = new()
        {
            OrderLines = @event.OrderLines.Select(ol => new OrderLine
            {
                ProductCatalogueNumber = ol.CatalogueNumber,
                Product = products.SingleOrDefault(p => p.CatalogueNumber == ol.CatalogueNumber),
                Count = ol.Count,
                TotalAmount = ol.Price
            }).ToList(),
        };
        orderEntity.Data = _serializer.Serialize(order);

        context.Add(orderEntity);
    }

    private async Task When(OrderPaymentSubmittedEvent @event, StoreShoppingDbContext context)
    {
        OrderEntity orderEntity = await context.Orders.FindAsync(@event.OrderId);
        if (orderEntity == null) return;
        
        UpdateOrder(context, orderEntity, order => order.PaymentId = @event.PaymentId);
    }

    private async Task When(OrderShippingInformationSetEvent @event, StoreShoppingDbContext context)
    {
        OrderEntity orderEntity = await context.Orders.FindAsync(@event.OrderId);
        if (orderEntity == null) return;
        
        UpdateOrder(context, orderEntity, order =>
        {
            Domain.Orders.ValueObjects.ShippingInfo shippingInfo = @event.ShippingInfo;
            order.ShippingInformation = new()
            {
                CountryCode = shippingInfo.CountryCode,
                FullName = shippingInfo.FullName,
                StreetAddress = shippingInfo.StreetAddress,
                City = shippingInfo.City,
                StateProvince = shippingInfo.StateProvince,
                Postcode = shippingInfo.Postcode,
                PhoneNumber = shippingInfo.PhoneNumber
            }; 
        });
    }

    private async Task When(OrderConfirmedEvent @event, StoreShoppingDbContext context)
    {
        OrderEntity orderEntity = await context.Orders.FindAsync(@event.OrderId);
        if (orderEntity == null) return;

        UpdateOrder(context, orderEntity, order => order.Status = OrderStatus.Confirmed);
    }

    private void UpdateOrder(StoreShoppingDbContext context, OrderEntity orderEntity, Action<Order> updateAction)
    {
        Order order = _serializer.Deserialize<Order>(orderEntity.Data);

        updateAction(order);
        
        orderEntity.Data = _serializer.Serialize(order);
        orderEntity.UpdatedAt = DateTime.UtcNow;

        context.Update(orderEntity);  
    }
}