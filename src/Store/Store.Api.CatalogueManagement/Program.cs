using System.Text.Json.Serialization;
using EventStore.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Integration;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EventStore;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Store.Api.CatalogueManagement", Version = "v1" }); });

builder.Services.AddSingleton(_ => new EventStoreEventDispatcherConfiguration
{
    IntegrationStreamName = "catalogue-integration"
});
builder.Services.AddSingleton<IEventDispatcher, EventStoreEventDispatcher>();
builder.Services.AddSingleton<IIntegrationEventMapper, CatalogueIntegrationEventMapper>();

builder.Services.AddSingleton<ISerializer, JsonSerializer>();

builder.Services.AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(builder.Configuration["EventStore:ConnectionString"])));
builder.Services.AddDbContext<StoreCatalogueDbContext>(
    options => options.UseNpgsql(builder.Configuration["Postgres:ConnectionString"], b => b.MigrationsAssembly("Store.Catalogue.Infrastructure")));

#endregion

#region App

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store.Api.CatalogueManagement v1"));
}

#if DEBUG

using (IServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = scope.ServiceProvider.GetService<StoreCatalogueDbContext>();
    context?.Database.Migrate();
}

#endif

app.UseHttpsRedirection();

app.UsePathBase(new PathString("/catalogue"));
app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion