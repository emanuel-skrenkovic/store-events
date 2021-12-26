using System;
using System.Collections.Generic;
using EventStore.Client;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure;
using Store.Core.Infrastructure.EventStore;
using Store.Core.Tests.Infrastructure;
using Store.Shopping.Application;
using Store.Shopping.Application.Buyers;
using Store.Shopping.Application.Buyers.Projections;
using Store.Shopping.Application.Orders.Commands.PlaceOrder;
using Store.Shopping.Application.Products;
using Store.Shopping.Application.Products.Projections;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Payments;
using Store.Shopping.Infrastructure;

namespace Store.Shopping.Tests;

public class StoreShoppingFixture : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    
    public EventStoreFixture EventStoreFixture { get; }
    public PostgresFixture<StoreOrderDbContext> PostgresFixture { get; }
    
    public StoreShoppingFixture()
    {
        EventStoreFixture = new();
        PostgresFixture = new();
        
        IServiceCollection services = new ServiceCollection();
        
        services.AddMediatR(typeof(OrderPlaceCommand));
        
        services.AddSingleton<EventStoreClient>(EventStoreFixture.EventStore);
        services.AddScoped<StoreOrderDbContext>(_ => PostgresFixture.Context);
        
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
        services.AddSingleton<EventStoreConnectionConfiguration>(_ => new EventStoreConnectionConfiguration
        {
            SubscriptionId = "projections"
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    public T GetService<T>() => _serviceProvider.GetRequiredService<T>();
    
    #region IDisposable
    
    private void ReleaseUnmanagedResources()
    {
        EventStoreFixture.Dispose();
        PostgresFixture.Dispose();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~StoreShoppingFixture()
    {
        ReleaseUnmanagedResources();
    }
    
    #endregion
}