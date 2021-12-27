using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Store.Catalogue.AspNet.Commands;
using Store.Catalogue.AspNet.Models;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;
using Store.Catalogue.Integration.Events;
using Store.Core.Domain.Event;
using Xunit;

namespace Store.Catalogue.Tests.Integration;

[Collection(StoreCatalogueFixtureCollection.Name)]
public class ApiTests : IAsyncLifetime
{
    private readonly StoreCatalogueFixture _fixture;

    public ApiTests(StoreCatalogueFixture fixture) 
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

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
        
        Guid productId = new Guid(response.Headers.Location.AbsolutePath.Split('/').Last());
        
        Assert.NotEqual(default, productId);
        
        StoreCatalogueDbContext context = _fixture.PostgresFixture.Context;
        ProductEntity productEntity = await context.FindAsync<ProductEntity>(productId);
        
        Assert.NotNull(productEntity);
        
        Assert.Equal(productEntity.Name, productName);
        Assert.Equal(productEntity.Price, productPrice);
        Assert.Equal(productEntity.Available, productAvailable);

        #endregion
    }

    [Fact]
    public async Task PostProduct_Should_CreateIntegrationEvent()
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

        List<IEvent> events = await _fixture.EventStoreFixture.Events("catalogue-integration");
        Assert.Contains(events, e => e is ProductCreatedEvent);

        var productCreatedEvent = events.FirstOrDefault(e => e is ProductCreatedEvent) as ProductCreatedEvent;
        Assert.NotNull(productCreatedEvent);
        
        Guid productId = new Guid(response.Headers.Location.AbsolutePath.Split('/').Last());
        Assert.Equal(productId, productCreatedEvent.ProductId);
        Assert.Equal(productName, productCreatedEvent.ProductView.Name);
        Assert.Equal(productPrice, productCreatedEvent.ProductView.Price);
        Assert.Equal(productAvailable, productCreatedEvent.ProductView.Available);
        
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
        Guid productId = new Guid(postResponse.Headers.Location.AbsolutePath.Split('/').Last());
        
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
        HttpResponseMessage response = await client.PutAsJsonAsync($"/products/{productId}", updateCommand);

        #endregion
        
        #region Assert
        
        Assert.True(response.IsSuccessStatusCode);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        StoreCatalogueDbContext context = _fixture.PostgresFixture.Context;
        ProductEntity productEntity = await context.FindAsync<ProductEntity>(productId);
        
        Assert.NotNull(productEntity);
        
        Assert.Equal(productEntity.Name, updatedProductName);
        Assert.Equal(productEntity.Price, updatedProductPrice);
        Assert.Equal(productEntity.Available, updatedProductAvailable);

        #endregion
    }
    
    [Fact]
    public async Task PutProduct_Should_CreateIntegrationEvent()
    {
        var client = _fixture.GetClient();

        const string productName = "product-name";
        decimal productPrice = 15;
        bool productAvailable = true;
        
        #region Preconditions
        
        ProductCreateCommand command = new(new ProductApiModel
        {
            Name = productName,
            Price = productPrice,
            Available = productAvailable
        });
        HttpResponseMessage postResponse = await client.PostAsJsonAsync("/products", command);
        Assert.True(postResponse.IsSuccessStatusCode);
        
        Guid productId = new Guid(postResponse.Headers.Location.AbsolutePath.Split('/').Last());
        
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
        HttpResponseMessage response = await client.PutAsJsonAsync($"/products/{productId}", updateCommand);
        
        #endregion
        
        #region Assert
        
        Assert.True(response.IsSuccessStatusCode);

        List<IEvent> events = await _fixture.EventStoreFixture.Events("catalogue-integration");
        Assert.Contains(events, e => e is ProductCreatedEvent);

        var productUpdatedEvent = events.FirstOrDefault(e => e is ProductUpdatedEvent) as ProductUpdatedEvent;
        Assert.NotNull(productUpdatedEvent);
        
        Assert.Equal(productId, productUpdatedEvent.ProductId);
        Assert.Equal(updatedProductName, productUpdatedEvent.ProductView.Name);
        Assert.Equal(updatedProductPrice, productUpdatedEvent.ProductView.Price);
        Assert.Equal(updatedProductAvailable, productUpdatedEvent.ProductView.Available);
        
        #endregion
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _fixture.EventStoreFixture.CleanAsync();
        await _fixture.PostgresFixture.CleanAsync();
    }
}