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
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Payments;
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
        
        if (!OpenPortsFinder.TryGetPort(new Range(31500, 32000), out int freePostgresPort))
        {
            throw new InvalidOperationException($"Could not find open port in {nameof(StoreShoppingCombinedFixture)}.");
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

    /*
    public async Task<Guid> OrderCreated(string customerNumber, string sessionId, params string[] productNumbers)
    {
        var mediator = _fixture.GetService<IMediator>();

        ProductEntity[] products = new ProductEntity[productNumbers.Length];
        for (int i = 0; i < productNumbers.Length; ++i) 
            products[i] = new ProductEntity { CatalogueNumber = productNumbers[i], Available = true, Name = $"Product_{i}" };
        await ProductsExist(products);
        await BuyerCreated(customerNumber, sessionId, productNumbers);

        OrderPlaceCommand orderPlaceCommand = new(customerNumber, sessionId);
        Result<OrderPlaceResponse> orderPlaceResult = await mediator.Send(orderPlaceCommand);

        return orderPlaceResult.Unwrap().OrderId;
    }
    */
    
    #region IAsyncLifetime
    
    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            PostgresFixture.InitializeAsync(), 
            EventStoreFixture.InitializeAsync());
        
        // IServiceCollection services = new ServiceCollection();
        //
        // services.AddMediatR(typeof(BuyerAddItemToCartCommand));
        //
        // services.AddScoped(_ => PostgresFixture.Context);
        //
        // services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
        // services.AddScoped<IOrderRepository, OrderRepository>();
        // services.AddScoped<IBuyerRepository, BuyerRepository>();
        // services.AddScoped<IPaymentRepository, PaymentRepository>();
        //
        // // services.AddTransient<ProductInfoService>();
        //
        // services.AddScoped<IOrderPaymentService, OrderPaymentService>();
        // services.AddScoped<CartReadService>();
        //
        // services.AddScoped<ISerializer, JsonSerializer>();
        // services.AddSingleton(_ => new EventStoreConnectionConfiguration
        // {
        //     SubscriptionId = "projections"
        // });
        //
        // services.AddSingleton(EventStoreFixture.EventStore);
        //
        // _serviceProvider = services.BuildServiceProvider(); 
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
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IBuyerRepository, BuyerRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // services.AddTransient<ProductInfoService>();

        services.AddScoped<IOrderPaymentService, OrderPaymentService>();
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