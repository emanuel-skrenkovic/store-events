using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Infrastructure.EventStore;
using Store.Core.Tests.Infrastructure;
using Store.Shopping.Application.Buyers;
using Store.Shopping.Application.Buyers.Commands.AddItemToCart;
using Store.Shopping.Infrastructure;
using Store.Shopping.Infrastructure.Entity;
using Xunit;

namespace Store.Shopping.Tests;

public class StoreShoppingCombinedFixture : IAsyncLifetime
{
    public EventStoreFixture EventStoreFixture { get; }

    public PostgresFixture<StoreShoppingDbContext> PostgresFixture { get; }
    
    public StoreShoppingCombinedFixture()
    {
        #region EventStore
        
        if (!OpenPortsFinder.TryGetPort(new Range(31000, 31500), out int freeEventStorePort))
        {
            throw new InvalidOperationException($"Could not find open port in {nameof(StoreShoppingCombinedFixture)}.");
        }
        
        EventStoreFixture = new(() => new EventStoreClient(
                EventStoreClientSettings.Create($"esdb://localhost:{freeEventStorePort}?tls=false&tlsVerifyCert=false")),
            new() { ["2113"] = freeEventStorePort.ToString() });
        
        #endregion
        
        #region Postgres
        
        if (!OpenPortsFinder.TryGetPort(new Range(31500, 32000), out int freePostgresPort))
        {
            throw new InvalidOperationException($"Could not find open port in {nameof(StoreShoppingCombinedFixture)}.");
        }
        
        string postgresConnectionString = $"User ID=postgres;Password=postgres;Server=localhost;Port={freePostgresPort};Database=store-catalogue;Integrated Security=true;Pooling=true;";

        PostgresFixture = new PostgresFixture<StoreShoppingDbContext>(
            () =>
            {
                DbContextOptionsBuilder<StoreShoppingDbContext> optionsBuilder = new();
                optionsBuilder
                    .UseNpgsql(postgresConnectionString);

                return new StoreShoppingDbContext(
                    optionsBuilder.Options);
            }, 
            new() { ["5432"] = freePostgresPort.ToString() });
        
        #endregion
    }
    
    public T GetService<T>() => BuildServices().GetRequiredService<T>();
    
    public async Task ProductsExist(params ProductEntity[] products)
    {
        var context = GetService<StoreShoppingDbContext>();

        foreach (ProductEntity product in products)
            context.Add(product);

        await context.SaveChangesAsync();
    }

    public Task BuyerCreated(string customerNumber, string sessionId, params string[] productCatalogueNumbers)
    {
        return Task.WhenAll(productCatalogueNumbers.Select(pn =>
        {
            var mediator = GetService<IMediator>();
            BuyerAddItemToCartCommand validRequest = new(
                customerNumber, 
                sessionId, 
                pn);
            return mediator.Send(validRequest); 
        }));
    }
    
    #region IAsyncLifetime
    
    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            PostgresFixture.InitializeAsync(), 
            EventStoreFixture.InitializeAsync());
    }

    private IServiceProvider BuildServices()
    {
        IServiceCollection services = new ServiceCollection();
        
        services.AddMediatR(typeof(BuyerAddItemToCartCommand));

        //services.AddScoped(_ => PostgresFixture.Context.Database.GetConnectionString());
        services.AddScoped(_ =>
        {
            DbContextOptionsBuilder<StoreShoppingDbContext> optionsBuilder = new();
            optionsBuilder
                .UseNpgsql(PostgresFixture.Context.Database.GetConnectionString());

            return new StoreShoppingDbContext(
                optionsBuilder.Options);
        });
        
        services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
        services.AddScoped<CartReadService>();
        
        services.AddScoped<ISerializer, JsonSerializer>();
        services.AddSingleton(_ => new EventStoreConnectionConfiguration
        {
            SubscriptionId = "projections"
        });
        
        services.AddSingleton(EventStoreFixture.EventStore);
        
        return services.BuildServiceProvider(); 
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            EventStoreFixture.DisposeAsync(), 
            PostgresFixture.DisposeAsync());
    }
    
    #endregion
}