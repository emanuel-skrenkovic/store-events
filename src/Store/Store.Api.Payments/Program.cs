using System.Text.Json.Serialization;
using EventStore.Client;
using MediatR;
using Store.Core.Domain;
using Store.Core.Infrastructure.EventStore;
using Store.Payments.Application.Payments.Commands;
using Store.Payments.Domain.Payments;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(builder.Configuration["EventStore:ConnectionString"])));

builder.Services.AddMediatR(typeof(PaymentCreateCommand));

builder.Services.AddSingleton<ISerializer, JsonSerializer>();

builder.Services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

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