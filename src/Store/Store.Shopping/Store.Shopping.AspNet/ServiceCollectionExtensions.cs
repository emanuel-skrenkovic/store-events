using EventStore.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure;
using Store.Core.Infrastructure.EventStore;
using Store.Shopping.Application.Buyers;
using Store.Shopping.Application.Buyers.Projections;
using Store.Shopping.Application.Orders.Commands.PlaceOrder;
using Store.Shopping.Application.Orders.Projections;
using Store.Shopping.Application.Products.Projections;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Infrastructure;

namespace Store.Shopping.AspNet;

public record ShoppingConfiguration
{
    public string EventStoreConnectionString { get; set; }
    
    public string PostgresConnectionString { get; set; }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShopping(this IServiceCollection services, Action<ShoppingConfiguration> configurationBuilder)
    {
        ShoppingConfiguration configuration = new();
        configurationBuilder(configuration);

        if (string.IsNullOrWhiteSpace(configuration.EventStoreConnectionString)) 
            throw new InvalidOperationException($"Cannot create module 'Shopping' if {configuration.EventStoreConnectionString} is null or empty.");
        if (string.IsNullOrWhiteSpace(configuration.PostgresConnectionString)) 
            throw new InvalidOperationException($"Cannot create module 'Shopping' if {configuration.EventStoreConnectionString} is null or empty.");

        services.AddMediatR(typeof(OrderPlaceCommand));

        services.AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(configuration.EventStoreConnectionString)));
            
        services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
 
        services.AddScoped<CartReadService>();

        services.AddSingleton<ISerializer, JsonSerializer>();
            
        services.AddDbContext<StoreShoppingDbContext>(
            options => options.UseNpgsql(
                configuration.PostgresConnectionString, 
                b => b.MigrationsAssembly("Store.Shopping.Infrastructure")));
        
        services.AddSingleton<IEventSubscriptionFactory, EventStoreSubscriptionFactory>();
        services.AddSingleton<IEventListener, CartProjection>();
        services.AddSingleton<IEventListener, ProductProjection>();
        services.AddSingleton<IEventListener, OrderProjection>();

        services.AddHostedService<EventStoreSubscriptionService>();

        return services;
    }
}