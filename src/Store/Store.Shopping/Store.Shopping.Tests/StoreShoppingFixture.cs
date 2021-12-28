using System;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EventStore;
using Store.Core.Tests.Infrastructure;
using Store.Shopping.Application;
using Store.Shopping.Application.Buyers;
using Store.Shopping.Application.Orders.Commands.PlaceOrder;
using Store.Shopping.Application.Products;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Payments;
using Store.Shopping.Infrastructure;
using Xunit;

namespace Store.Shopping.Tests;

public class StoreShoppingFixture : IAsyncLifetime
{
    private IServiceProvider _serviceProvider;
    
    public EventStoreFixture EventStoreFixture { get; }
    public PostgresFixture<StoreOrderDbContext> PostgresFixture { get; }
    
    public StoreShoppingFixture()
    {
        EventStoreFixture = new();
        PostgresFixture = new();
    }

    public T GetService<T>() => _serviceProvider.GetRequiredService<T>();
    
    public async Task InitializeAsync()
    {
        await EventStoreFixture.InitializeAsync();
        await PostgresFixture.InitializeAsync();
        
        IServiceCollection services = new ServiceCollection();
        
        services.AddMediatR(typeof(OrderPlaceCommand));
        
        services.AddSingleton(EventStoreFixture.EventStore);
        services.AddScoped(_ => PostgresFixture.Context);
        
        services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IBuyerRepository, BuyerRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<ProductInfoService>();
        services.AddScoped<IBuyerOrderService, BuyerOrderService>();
        services.AddScoped<IOrderPaymentService, OrderPaymentService>();
        services.AddScoped<CartReadService>();
        
        services.AddScoped<IEventSubscriptionFactory, EventStoreSubscriptionFactory>();
        services.AddScoped<ISerializer, JsonSerializer>();
        services.AddSingleton(_ => new EventStoreConnectionConfiguration
        {
            SubscriptionId = "projections"
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task DisposeAsync()
    {
        await EventStoreFixture.DisposeAsync();
        await PostgresFixture.DisposeAsync();
    }
}