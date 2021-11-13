using System.Text.Json.Serialization;
using EventStore.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure;
using Store.Core.Infrastructure.EventStore;
using Store.Order.Application.Buyer.Projections.Cart;
using Store.Order.Application.Order.Commands.PlaceOrder;
using Store.Order.Domain;
using Store.Order.Domain.Buyers;
using Store.Order.Domain.Orders;
using Store.Order.Infrastructure;
using Store.Order.Infrastructure.Entity;

#region Services

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<ICustomerOrderService, CustomerOrderService>();

builder.Services.AddSingleton<ISerializer, JsonSerializer>();
            
builder.Services.AddDbContext<StoreOrderDbContext>(
    options => options.UseNpgsql(builder.Configuration["Postgres:ConnectionString"], b => b.MigrationsAssembly("Store.Order.Infrastructure")));

#region ProjectionServices

builder.Services.AddSingleton(_ => new EventStoreConnectionConfiguration
{
    SubscriptionId = "projections"
});

builder.Services.AddSingleton<IEventSubscriptionFactory, EventStoreSubscriptionFactory>();
builder.Services.AddSingleton<IProjection<CartEntity, StoreOrderDbContext>, CartProjection>();
builder.Services.AddSingleton<IProjectionManager, CartProjectionManager>();

builder.Services.AddHostedService<EventStoreSubscriptionService>();

#endregion

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();