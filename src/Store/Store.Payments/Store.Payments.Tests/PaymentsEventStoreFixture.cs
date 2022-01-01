using System;
using System.Threading.Tasks;
using EventStore.Client;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Core.Infrastructure.EventStore;
using Store.Core.Tests.Infrastructure;
using Store.Payments.Application.Payments.Commands;
using Store.Payments.Domain.Payments;
using Xunit;

namespace Store.Payments.Tests;

public class PaymentsEventStoreFixture : IAsyncLifetime
{
    public EventStoreFixture EventStoreFixture { get; }

    public PaymentsEventStoreFixture()
    {
        if (!OpenPortsFinder.TryGetPort(new Range(32000, 32500), out int freeEventStorePort))
        {
            throw new InvalidOperationException($"Could not find open port in {nameof(PaymentsEventStoreFixture)}.");
        }
        
        EventStoreFixture = new(() => new EventStoreClient(
                EventStoreClientSettings.Create($"esdb://localhost:{freeEventStorePort}?tls=false&tlsVerifyCert=false")),
            new() { ["2113"] = freeEventStorePort.ToString() });
    }
    
    public T GetService<T>() => BuildServiceProvider().GetRequiredService<T>();

    public async Task<Guid> PaymentExists(string source, decimal amount, string note)
    {
        var mediator = GetService<IMediator>();
        
        PaymentCreateCommand command = new(source, amount, note);
        Result<PaymentCreateResponse> paymentCreateResult = await mediator.Send(command);
        
        return paymentCreateResult.Unwrap().PaymentId;
    }

    private IServiceProvider BuildServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddMediatR(typeof(PaymentCreateCommand));
        services.AddSingleton(EventStoreFixture.EventStore);

        services.AddSingleton<ISerializer, JsonSerializer>();
        
        services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        
        return services.BuildServiceProvider();
    }
    
    #region IAsyncLifetime
    
    public Task InitializeAsync() => EventStoreFixture.InitializeAsync();
    public Task DisposeAsync() => EventStoreFixture.DisposeAsync();

    #endregion
}