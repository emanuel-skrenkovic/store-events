using EventStore.Client;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Infrastructure.EventStore;
using Store.Payments.Application.Payments.Commands;
using Store.Payments.Domain.Payments;

namespace Store.Payments.AspNet;

public class PaymentsConfiguration
{
    public string EventStoreConnectionString { get; set; }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPayments(this IServiceCollection services, Action<PaymentsConfiguration> configurationBuilder)
    {
        PaymentsConfiguration configuration = new();
        configurationBuilder(configuration);

        if (string.IsNullOrWhiteSpace(configuration.EventStoreConnectionString))
            throw new InvalidOperationException($"Cannot create module 'Payments' if {configuration.EventStoreConnectionString} is null or empty.");
        
        services.AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(configuration.EventStoreConnectionString)));

        services.AddMediatR(typeof(PaymentCreateCommand));

        services.AddSingleton<ISerializer, JsonSerializer>();

        services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}