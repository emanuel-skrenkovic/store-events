using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Catalogue.AspNet.Commands;
using Store.Catalogue.AspNet.Models;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;
using Xunit;

namespace Store.Catalogue.Tests.Integration;

public class DatabaseTests : IClassFixture<StoreCatalogueDatabaseFixture>
{
    private readonly StoreCatalogueDatabaseFixture _fixture;

    public DatabaseTests(StoreCatalogueDatabaseFixture fixture) 
        => _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

    [Fact]
    public async Task PostProduct_Should_CreateNewProductEntry()
    {
        var client = _fixture.GetClient();

        const string productName = "product-name";
        decimal productPrice = 15;
        bool productAvailable = true;
        
        #region Act
        
        ProductCreateCommand command = new(new ProductApiModel
        {
            Name = productName,
            Price = productPrice,
            Available = productAvailable
        });
        HttpResponseMessage response = await client.PostAsJsonAsync("/products", command);
        
        #endregion
        
        #region Assert
        
        Assert.True(response.IsSuccessStatusCode);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Contains(response.Headers, kv => kv.Key == "Location");
        Assert.NotNull(response.Headers.Location);
        
        Guid productCatalogueId = new Guid(response.Headers.Location.AbsolutePath.Split('/').Last());
        
        Assert.NotEqual(default, productCatalogueId);
        
        StoreCatalogueDbContext context = _fixture.PostgresFixture.Context;
        ProductEntity productEntity = await context.Products.SingleOrDefaultAsync(p => p.CatalogueId == productCatalogueId);
        
        Assert.NotNull(productEntity);
        
        Assert.Equal(productEntity.Name, productName);
        Assert.Equal(productEntity.Price, productPrice);
        Assert.Equal(productEntity.Available, productAvailable);

        #endregion
    }

    [Fact]
    public async Task PutProduct_Should_UpdateExistingProductEntry()
    {
        var client = _fixture.GetClient();
        
        #region Preconditions
        
        const string productName = "product-name";
        decimal productPrice = 15;
        bool productAvailable = true;
        
        ProductCreateCommand command = new(new ProductApiModel
        {
            Name = productName,
            Price = productPrice,
            Available = productAvailable
        });
        HttpResponseMessage postResponse = await client.PostAsJsonAsync("/products", command);
        Guid productCatalogueId = new Guid(postResponse.Headers.Location.AbsolutePath.Split('/').Last());
        
        #endregion
        
        #region Act
        
        const string updatedProductName = "updated-product-name";
        decimal updatedProductPrice = 17;
        bool updatedProductAvailable = false;

        ProductUpdateCommand updateCommand = new(new ProductApiModel{
            Name = updatedProductName,
            Price = updatedProductPrice,
            Available = updatedProductAvailable
        });
        HttpResponseMessage response = await client.PutAsJsonAsync($"/products/{productCatalogueId}", updateCommand);

        #endregion
        
        #region Assert
        
        Assert.True(response.IsSuccessStatusCode);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        StoreCatalogueDbContext context = _fixture.PostgresFixture.Context;
        ProductEntity productEntity = await context.Products.SingleOrDefaultAsync(p => p.CatalogueId == productCatalogueId);
        
        Assert.NotNull(productEntity);
        
        Assert.Equal(updatedProductName, productEntity.Name);
        Assert.Equal(updatedProductPrice, productEntity.Price);
        Assert.Equal(updatedProductAvailable, productEntity.Available);

        #endregion
    }
}