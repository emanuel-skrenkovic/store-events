using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Infrastructure;
using Store.Core.Domain.Event;
using Store.Core.Tests.Infrastructure;

namespace Store.Catalogue.Tests;

public class StoreCatalogueFixture : IDisposable
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
        PostgresFixture = new PostgresFixture<StoreCatalogueDbContext>(
            () =>
            {
                DbContextOptionsBuilder<StoreCatalogueDbContext> optionsBuilder = new();
                optionsBuilder.UseNpgsql(PostgresConnectionString);
                
                return new StoreCatalogueDbContext(
                    optionsBuilder.Options,
                    _webApplicationFactory.Services.GetRequiredService<IIntegrationEventMapper>(),
                    _webApplicationFactory.Services.GetRequiredService<IEventDispatcher>());
            });
    }

    public T GetService<T>() => _webApplicationFactory.Services.GetRequiredService<T>();

    public HttpClient GetClient() => _webApplicationFactory.CreateClient();
    
    #region IDisposable

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _webApplicationFactory.Dispose();
            PostgresFixture?.Dispose();
            EventStoreFixture?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~StoreCatalogueFixture()
    {
        Dispose(false);
    }
    
    #endregion
}