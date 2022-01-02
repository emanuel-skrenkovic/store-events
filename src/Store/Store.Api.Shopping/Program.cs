using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Store.Shopping.AspNet;
using Store.Shopping.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services
    .AddShopping(configuration =>
    {
        configuration.EventStoreConnectionString = builder.Configuration["EventStore:ConnectionString"];
        configuration.PostgresConnectionString   = builder.Configuration["Postgres:ConnectionString"];
    })
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

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