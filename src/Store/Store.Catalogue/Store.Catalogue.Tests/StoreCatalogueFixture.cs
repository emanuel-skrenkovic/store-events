using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Infrastructure;
using Store.Core.Domain.Event;
using Store.Core.Tests.Infrastructure;
using Xunit;

namespace Store.Catalogue.Tests;

public class StoreCatalogueFixture : IAsyncLifetime
{
    private const string PostgresConnectionString = 
        "User ID=postgres;Password=postgres;Server=localhost;Port=5432;Database=store-catalogue;Integrated Security=true;Pooling=true;";
    
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public EventStoreFixture EventStoreFixture { get; }
    public PostgresFixture<StoreCatalogueDbContext> PostgresFixture { get; }
    
    public StoreCatalogueFixture()
    {
        _webApplicationFactory = new WebApplicationFactory<Program>();

        EventStoreFixture = new();
        PostgresFixture = new PostgresFixture<StoreCatalogueDbContext>();
        PostgresFixture.ContextFactory = () =>
            {
                DbContextOptionsBuilder<StoreCatalogueDbContext> optionsBuilder = new();
                optionsBuilder.UseNpgsql(PostgresConnectionString);
                
                return new StoreCatalogueDbContext(
                    optionsBuilder.Options,
                    _webApplicationFactory.Services.GetRequiredService<IIntegrationEventMapper>(),
                    _webApplicationFactory.Services.GetRequiredService<IEventDispatcher>());
            };
    }

    public T GetService<T>() => _webApplicationFactory.Services.GetRequiredService<T>();

    public HttpClient GetClient() => _webApplicationFactory.CreateClient();
    
    #region IAsyncLifetime

    public async Task InitializeAsync()
    {
        await PostgresFixture.InitializeAsync();
        await EventStoreFixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await _webApplicationFactory.DisposeAsync();
        await PostgresFixture.DisposeAsync();
        await EventStoreFixture.DisposeAsync();
    }
    
    #endregion
}