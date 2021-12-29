using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

public class StoreShoppingEventStoreFixture : IAsyncLifetime
{
    private IServiceProvider _serviceProvider;
    
    public EventStoreFixture EventStoreFixture { get; }

    public StoreShoppingEventStoreFixture()
    {
        EventStoreFixture = new();
    }
    
    public T GetService<T>() => _serviceProvider.GetRequiredService<T>();
    
    #region IAsyncLifetime

    public async Task InitializeAsync()
    {
        IServiceCollection services = new ServiceCollection();
        
        services.AddMediatR(typeof(OrderPlaceCommand));
        
        services.AddDbContext<StoreShoppingDbContext>(
            options => options.UseInMemoryDatabase("store-shopping"));
        
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
        
        await EventStoreFixture.InitializeAsync();
        services.AddSingleton(EventStoreFixture.EventStore);
        
        _serviceProvider = services.BuildServiceProvider();
    }

    public Task DisposeAsync() => EventStoreFixture.DisposeAsync();
    
    #endregion
}