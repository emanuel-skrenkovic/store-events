using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EntityFramework.Extensions;

namespace Store.Catalogue.Application.Product.Projections;

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
        
    #region EventListener
        
    public async Task StartAsync()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        StoreCatalogueDbContext context = scope.ServiceProvider.GetRequiredService<StoreCatalogueDbContext>();

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

    #endregion
        
    #region Projection

    public async Task ProjectAsync(IEvent receivedEvent, EventMetadata eventMetadata)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        StoreCatalogueDbContext context = scope.ServiceProvider.GetRequiredService<StoreCatalogueDbContext>();
        if (context == null) return; // TODO: error or log?
                
        Func<Task> projectionAction = receivedEvent switch
        {
            ProductCreatedEvent @event           => () => When(@event, context),
            ProductPriceChangedEvent @event      => () => When(@event, context),
            ProductRenamedEvent @event           => () => When(@event, context),
            ProductMarkedAvailableEvent @event   => () => When(@event, context),
            ProductMarkedUnavailableEvent @event => () => When(@event, context),
            _ => null
        };

        if (projectionAction == null) return;

        await projectionAction();
        await context.AddOrUpdateSubscriptionCheckpoint(SubscriptionId, eventMetadata.StreamPosition);
            
        await context.SaveChangesAsync();
    }
        
    private Task When(ProductCreatedEvent @event, StoreCatalogueDbContext context)
    {
        ProductEntity product = new()
        {
            Id = @event.EntityId,
            Name = @event.Name,
            Price = @event.Price,
            Description = @event.Description,
            Available = false
        };
            
        context.Add(product);

        return Task.CompletedTask;
    }
        
    private async Task When(ProductPriceChangedEvent @event, StoreCatalogueDbContext context)
    {
        ProductEntity product = await context.FindAsync<ProductEntity>(@event.EntityId);
        if (product == null) return;

        product.Price = @event.NewPrice;
            
        context.Update(product);
    }
        
    private async Task When(ProductRenamedEvent @event, StoreCatalogueDbContext context)
    {
        ProductEntity product = await context.FindAsync<ProductEntity>(@event.EntityId);
        if (product == null) return;

        product.Name = @event.NewName;
            
        context.Update(product);
    }
        
    private async Task When(ProductMarkedAvailableEvent @event, StoreCatalogueDbContext context)
    {
        ProductEntity product = await context.FindAsync<ProductEntity>(@event.EntityId);
        if (product == null) return;
            
        product.Available = true;
            
        context.Update(product);
    }
        
    private async Task When(ProductMarkedUnavailableEvent @event, StoreCatalogueDbContext context)
    {
        ProductEntity product = await context.FindAsync<ProductEntity>(@event.EntityId);
        if (product == null) return;

        product.Available = false;
            
        context.Update(product);
    }
        
    #endregion
}