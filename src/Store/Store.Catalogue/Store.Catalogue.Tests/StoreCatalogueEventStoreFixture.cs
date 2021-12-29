using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Infrastructure;
using Store.Core.Tests.Infrastructure;
using Xunit;

namespace Store.Catalogue.Tests;

public class StoreCatalogueEventStoreFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    public EventStoreFixture EventStoreFixture { get; }

    public StoreCatalogueEventStoreFixture()
    {
        EventStoreFixture = new();
        _webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Remove existing DbContext before adding the in-memory one.
                    ServiceDescriptor descriptor = services
                        .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StoreCatalogueDbContext>));
                    if (descriptor != null) services.Remove(descriptor);
                    
                    services.AddDbContext<StoreCatalogueDbContext>(
                        options => options.UseInMemoryDatabase("store-catalogue"));
                });
            });
    }
    
    public HttpClient GetClient() => _webApplicationFactory.CreateClient();
    
    #region IAsyncLifetime
    
    public Task InitializeAsync() => EventStoreFixture.InitializeAsync();

    public async Task DisposeAsync()
    {
        await _webApplicationFactory.DisposeAsync();
        await EventStoreFixture.DisposeAsync();
    }
    
    #endregion
}