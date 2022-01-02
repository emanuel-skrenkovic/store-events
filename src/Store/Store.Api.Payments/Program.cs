using System.Text.Json.Serialization;
using Store.Payments.AspNet;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddPayments(configuration => configuration.EventStoreConnectionString = builder.Configuration["EventStore:ConnectionString"])
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

#endregion

#region App

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();

#endregion