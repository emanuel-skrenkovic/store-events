using System;
using System.Threading.Tasks;
using EventStore.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Infrastructure.EventStore;
using Store.Core.Tests.Infrastructure;
using Store.Shopping.Application;
using Store.Shopping.Application.Buyers;
using Store.Shopping.Application.Buyers.Commands.AddItemToCart;
using Store.Shopping.Application.Products;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Payments;
using Store.Shopping.Infrastructure;
using Xunit;

namespace Store.Shopping.Tests;

public class StoreShoppingCombinedFixture : IAsyncLifetime
{
    private IServiceProvider _serviceProvider;
    
    public EventStoreFixture EventStoreFixture { get; }

    public PostgresFixture<StoreShoppingDbContext> PostgresFixture { get; }
    
    public StoreShoppingCombinedFixture()
    {
        #region EventStore
        
        if (!OpenPortsFinder.TryGetPort(new Range(31000, 31500), out int freeEventStorePort))
        {
            throw new InvalidOperationException($"Could not find open port in {nameof(StoreShoppingEventStoreFixture)}.");
        }
        
        EventStoreFixture = new(() => new EventStoreClient(
                EventStoreClientSettings.Create($"esdb://localhost:{freeEventStorePort}?tls=false&tlsVerifyCert=false")),
            new() { ["2113"] = freeEventStorePort.ToString() });
        
        if (!OpenPortsFinder.TryGetPort(new Range(31500, 32000), out int freePostgresPort))
        {
            throw new InvalidOperationException($"Could not find open port in {nameof(StoreShoppingEventStoreFixture)}.");
        }
        
        #endregion
        
        #region Postgres
        
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
    
    public T GetService<T>() => _serviceProvider.GetRequiredService<T>();
    
    #region IAsyncLifetime
    
    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            PostgresFixture.InitializeAsync(), 
            EventStoreFixture.InitializeAsync());
        
        IServiceCollection services = new ServiceCollection();
        
        services.AddMediatR(typeof(BuyerAddItemToCartCommand));

        services.AddScoped(_ => PostgresFixture.Context);
        
        services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IBuyerRepository, BuyerRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<IOrderPaymentService, OrderPaymentService>();
        services.AddScoped<CartReadService>();
        
        services.AddScoped<ISerializer, JsonSerializer>();
        services.AddSingleton(_ => new EventStoreConnectionConfiguration
        {
            SubscriptionId = "projections"
        });
        
        services.AddSingleton(EventStoreFixture.EventStore);
        
        _serviceProvider = services.BuildServiceProvider(); 
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            EventStoreFixture.DisposeAsync(), 
            PostgresFixture.DisposeAsync());
    }
    
    #endregion
}