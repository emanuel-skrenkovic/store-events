using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Store.Catalogue.AspNet.Commands;
using Store.Catalogue.AspNet.Models;
using Store.Catalogue.Integration.Events;
using Store.Core.Domain.Event;
using Xunit;

namespace Store.Catalogue.Tests.Integration;

public class IntegrationEventTests : IClassFixture<StoreCatalogueEventStoreFixture>, IAsyncLifetime
{
    private readonly StoreCatalogueEventStoreFixture _fixture;

    public IntegrationEventTests(StoreCatalogueEventStoreFixture fixture)
        => _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    
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

        List<(IEvent @event, EventMetadata metadata)> eventsAndMetadata = await _fixture.EventStoreFixture.EventsWithMetadata("catalogue-integration");
        List<IEvent> events = eventsAndMetadata.Select(em => em.@event).ToList();
        Assert.Contains(events, e => e is ProductCreatedEvent);

        var productCreatedEvent = events.FirstOrDefault(e => e is ProductCreatedEvent) as ProductCreatedEvent;
        Assert.NotNull(productCreatedEvent);
        
        Guid productId = new Guid(response.Headers.Location.AbsolutePath.Split('/').Last());
        Assert.Equal(productId, productCreatedEvent.ProductId);
        Assert.Equal(productName, productCreatedEvent.ProductView.Name);
        Assert.Equal(productPrice, productCreatedEvent.ProductView.Price);
        Assert.Equal(productAvailable, productCreatedEvent.ProductView.Available);
        
        EventMetadata eventMetadata = eventsAndMetadata.SingleOrDefault(e => e.@event is ProductCreatedEvent).metadata;
        Assert.NotNull(eventMetadata);
        Assert.NotEqual(default, eventMetadata.CorrelationId);
        Assert.NotEqual(default, eventMetadata.CausationId);
        
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

        List<(IEvent @event, EventMetadata metadata)> eventsAndMetadata = await _fixture.EventStoreFixture.EventsWithMetadata("catalogue-integration");
        List<IEvent> events = eventsAndMetadata.Select(em => em.@event).ToList();
        Assert.Contains(events, e => e is ProductCreatedEvent);

        var productUpdatedEvent = events.FirstOrDefault(e => e is ProductUpdatedEvent) as ProductUpdatedEvent;
        Assert.NotNull(productUpdatedEvent);
        
        Assert.Equal(productId, productUpdatedEvent.ProductId);
        Assert.Equal(updatedProductName, productUpdatedEvent.ProductView.Name);
        Assert.Equal(updatedProductPrice, productUpdatedEvent.ProductView.Price);
        Assert.Equal(updatedProductAvailable, productUpdatedEvent.ProductView.Available);

        EventMetadata eventMetadata = eventsAndMetadata.SingleOrDefault(e => e.@event is ProductUpdatedEvent).metadata;
        Assert.NotNull(eventMetadata);
        Assert.NotEqual(default, eventMetadata.CorrelationId);
        Assert.NotEqual(default, eventMetadata.CausationId);

        #endregion
    }
    
    #region IAsyncLifetime

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask; //_fixture.EventStoreFixture.CleanAsync();

    #endregion
}