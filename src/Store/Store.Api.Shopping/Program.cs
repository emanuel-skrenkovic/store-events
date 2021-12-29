using System.Text.Json.Serialization;
using EventStore.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure;
using Store.Core.Infrastructure.EventStore;
using Store.Shopping.Application;
using Store.Shopping.Application.Buyers;
using Store.Shopping.Application.Buyers.Projections;
using Store.Shopping.Application.Orders.Commands.PlaceOrder;
using Store.Shopping.Application.Orders.Projections;
using Store.Shopping.Application.Products;
using Store.Shopping.Application.Products.Projections;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Payments;
using Store.Shopping.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

#region Services

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(typeof(OrderPlaceCommand));

builder.Services.AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(builder.Configuration["EventStore:ConnectionString"])));
            
builder.Services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IBuyerRepository, BuyerRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IOrderPaymentService, OrderPaymentService>();
builder.Services.AddScoped<CartReadService>();

builder.Services.AddSingleton<ISerializer, JsonSerializer>();
            
builder.Services.AddDbContext<StoreShoppingDbContext>(
    options => options.UseNpgsql(builder.Configuration["Postgres:ConnectionString"], b => b.MigrationsAssembly("Store.Shopping.Infrastructure")));

#region ProjectionServices

builder.Services.AddSingleton(_ => new EventStoreConnectionConfiguration
{
    SubscriptionId = "projections"
});

builder.Services.AddSingleton<IEventSubscriptionFactory, EventStoreSubscriptionFactory>();
builder.Services.AddSingleton<IEventListener, CartProjection>();
builder.Services.AddSingleton<IEventListener, ProductProjection>();
builder.Services.AddSingleton<IEventListener, OrderProjection>();

builder.Services.AddHostedService<EventStoreSubscriptionService>();

#endregion

#endregion

#region App

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#if DEBUG

using (IServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = scope.ServiceProvider.GetService<StoreShoppingDbContext>();
    context?.Database.Migrate();
}

#endif

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion