using System;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Integration;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EventStore;

namespace Store.Catalogue.AspNet;

public class CatalogueConfiguration
{
    public string EventStoreConnectionString { get; set; }
    
    public string PostgresConnectionString { get; set; }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogue(this IServiceCollection services, Action<CatalogueConfiguration> configurationBuilder)
    {
        CatalogueConfiguration configuration = new();
        configurationBuilder(configuration);
        
        if (string.IsNullOrWhiteSpace(configuration.EventStoreConnectionString)) 
            throw new InvalidOperationException($"Cannot create module 'Catalogue' if {configuration.EventStoreConnectionString} is null or empty.");
        if (string.IsNullOrWhiteSpace(configuration.PostgresConnectionString)) 
            throw new InvalidOperationException($"Cannot create module 'Catalogue' if {configuration.PostgresConnectionString} is null or empty.");
        
        services.AddSingleton(_ => new EventStoreEventDispatcherConfiguration
        {
            IntegrationStreamName = "catalogue-integration"
        });

        services.AddSingleton(new EventStoreConnectionConfiguration { SubscriptionId = "Store.Catalogue" });
        
        services.AddSingleton<IEventDispatcher, EventStoreEventDispatcher>();
        services.AddSingleton<IIntegrationEventMapper, CatalogueIntegrationEventMapper>();

        services.AddSingleton<ISerializer, JsonSerializer>();

        services.AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(configuration.EventStoreConnectionString)));
        services.AddDbContext<StoreCatalogueDbContext>(
            options => options.UseNpgsql(
                configuration.PostgresConnectionString, 
                b => b.MigrationsAssembly("Store.Catalogue.Infrastructure")));

        return services;
    }
}