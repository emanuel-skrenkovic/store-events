using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Store.Catalogue.AspNet;
using Store.Catalogue.Infrastructure;
using Store.Core.Infrastructure.AspNet;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services
    .AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Store.Api.CatalogueManagement", Version = "v1" }); })
    .AddCatalogue(configuration =>
    {
        configuration.EventStoreConnectionString = builder.Configuration["EventStore:ConnectionString"];
        configuration.PostgresConnectionString   = builder.Configuration["Postgres:ConnectionString"];
    })
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        var jsonSerializerOptions = opts.JsonSerializerOptions;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

#endregion

#region App

var app = builder.Build();
app.UseMiddleware<CorrelationMiddleware>();

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
    if (context?.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
    {
        context?.Database.Migrate();
    }
} 

#endif

app.UseHttpsRedirection();
app.UsePathBase(new PathString("/catalogue"));
app.UseAuthorization();
app.MapControllers();

app.Run();

#endregion