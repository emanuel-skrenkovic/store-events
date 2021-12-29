using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Store.Catalogue.Infrastructure;
using Store.Core.Domain.Event;
using Store.Core.Tests.Infrastructure;
using Xunit;

namespace Store.Catalogue.Tests;

public class StoreCatalogueDatabaseFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    
    public PostgresFixture<StoreCatalogueDbContext> PostgresFixture { get; }
    
    public StoreCatalogueDatabaseFixture()
    {
        if (!OpenPortsFinder.TryGetPort(new System.Range(29000, 30000), out int freePort))
        {
            throw new InvalidOperationException($"Could not find open port in {nameof(StoreCatalogueDatabaseFixture)}.");
        }
        
        Mock<IIntegrationEventMapper> integrationEventMapperMock = new();
        IEvent integrationEvent = null;
        integrationEventMapperMock
            .Setup(m => m.TryMap(It.IsAny<object>(), out integrationEvent))
            .Returns(false);

        Mock<IEventDispatcher> eventDispatcherMock = new();
        eventDispatcherMock
            .Setup(ed => ed.DispatchAsync(It.IsAny<object>()))
            .Returns(Task.CompletedTask);

        string postgresConnectionString = $"User ID=postgres;Password=postgres;Server=localhost;Port={freePort};Database=store-catalogue;Integrated Security=true;Pooling=true;";

        PostgresFixture = new PostgresFixture<StoreCatalogueDbContext>(
            () =>
            {
                DbContextOptionsBuilder<StoreCatalogueDbContext> optionsBuilder = new();
                optionsBuilder
                    .UseNpgsql(postgresConnectionString);

                return new StoreCatalogueDbContext(
                    optionsBuilder.Options,
                    integrationEventMapperMock.Object,
                    eventDispatcherMock.Object);
            }, 
            new() { ["5432"] = freePort.ToString() });
        
        _webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, _) =>
                {
                    context.Configuration["Postgres:ConnectionString"] = postgresConnectionString;
                });
                    
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(integrationEventMapperMock.Object);
                    services.AddSingleton(eventDispatcherMock.Object);
                });
            });
    }

    public HttpClient GetClient() => _webApplicationFactory.CreateClient();
    
    #region IAsyncLifetime

    public async Task InitializeAsync()
    {
        await PostgresFixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await _webApplicationFactory.DisposeAsync();
        await PostgresFixture.DisposeAsync();
    }
    
    #endregion
}