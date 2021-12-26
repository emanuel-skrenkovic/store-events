using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Integration;
using Store.Catalogue.Integration.Events;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EntityFramework.Extensions;
using Store.Shopping.Infrastructure;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Products.Projections;

public class ProductProjection : IEventListener
{
    private const string SubscriptionId = nameof(ProductEntity);
        
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventSubscriptionFactory _eventSubscriptionFactory;

    public ProductProjection(
        IServiceScopeFactory scopeFactory,
        IEventSubscriptionFactory eventSubscriptionFactory)
    {
        _scopeFactory             = scopeFactory             ?? throw new ArgumentNullException(nameof(scopeFactory));
        _eventSubscriptionFactory = eventSubscriptionFactory ?? throw new ArgumentNullException(nameof(eventSubscriptionFactory));
    }
        
    public async Task StartAsync()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        StoreOrderDbContext context = scope.ServiceProvider.GetRequiredService<StoreOrderDbContext>();

        if (context == null)
        {
            throw new InvalidOperationException($"Context cannot be null on {nameof(ProductProjection)} startup.");
        }
            
        ulong checkpoint = await context.GetSubscriptionCheckpoint(SubscriptionId);
            
        await _eventSubscriptionFactory
            .Create(SubscriptionId, ProjectAsync)
            .SubscribeAtAsync(checkpoint);
    }

    public Task StopAsync()
    {
        // TODO: nothing needed
        return Task.CompletedTask;
    }
        
    private async Task ProjectAsync(IEvent receivedEvent, EventMetadata eventMetadata)
    {
        Ensure.NotNull(receivedEvent, nameof(receivedEvent));

        using IServiceScope scope = _scopeFactory.CreateScope();

        StoreOrderDbContext context = scope.ServiceProvider.GetRequiredService<StoreOrderDbContext>();
        if (context == null) return;

        Func<Task> projectionAction = receivedEvent switch
        {
            ProductCreatedEvent @event => () => When(@event, context),
            ProductUpdatedEvent @event => () => When(@event, context),
            _ => null
        };
        if (projectionAction == null) return;

        await projectionAction();
        await context.AddOrUpdateSubscriptionCheckpoint(SubscriptionId, eventMetadata.StreamPosition);

        await context.SaveChangesAsync();
    }
        
    private Task When(ProductCreatedEvent @event, StoreOrderDbContext context)
    {
        ProductView productView = @event.ProductView;
        context.Products.Add(new()
        {
            CatalogueNumber = @event.ProductId.ToString(),
            Name = productView.Name,
            Price = productView.Price,
            Available = productView.Available
        });

        return Task.CompletedTask;
    }
        
    private async Task When(ProductUpdatedEvent @event, StoreOrderDbContext context)
    {
        ProductEntity productEntity = await context.FindAsync<ProductEntity>(@event.ProductId);
        if (productEntity == null) return;

        ProductView productView = @event.ProductView;

        productEntity.Name = productView.Name;
        productEntity.Price = productView.Price;
        productEntity.Available = productView.Available;
            
        context.Update(productEntity);
    }
}