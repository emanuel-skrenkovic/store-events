using System;
using System.Threading.Tasks;
using EventStore.Client;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Infrastructure.EventStore;
using Store.Core.Tests.Infrastructure;
using Store.Inventory.Application.Commands;
using Xunit;

namespace Store.Inventory.Tests;

public class InventoryEventStoreFixture : IAsyncLifetime
{
    public EventStoreFixture EventStoreFixture { get; }

    public InventoryEventStoreFixture()
    {
        if (!OpenPortsFinder.TryGetPort(new Range(32500, 33000), out int freeEventStorePort))
        {
            throw new InvalidOperationException($"Could not find open port in {nameof(InventoryEventStoreFixture)}.");
        }
        
        EventStoreFixture = new(() => new EventStoreClient(
                EventStoreClientSettings.Create($"esdb://localhost:{freeEventStorePort}?tls=false&tlsVerifyCert=false")),
            new() { ["2113"] = freeEventStorePort.ToString() });
    }

    public T GetService<T>() => BuildServiceProvider().GetRequiredService<T>();

    private IServiceProvider BuildServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddMediatR(typeof(ProductInventoryAddToStockCommand));
        services.AddSingleton(EventStoreFixture.EventStore);

        services.AddSingleton<ISerializer, JsonSerializer>();
        
        services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
        
        return services.BuildServiceProvider();
    }
        
    #region IAsyncLifetime
    
    public Task InitializeAsync() => EventStoreFixture.InitializeAsync();
    public Task DisposeAsync() => EventStoreFixture.DisposeAsync();

    #endregion
}