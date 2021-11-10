using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Catalogue.Domain.Product.Events;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public class ProductDisplayProjection : IProjection<ProductDisplayEntity>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ProductDisplayProjection(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }
        
        public Func<Task> Project(IEvent receivedEvent)
        {
            return receivedEvent switch
            {
                ProductCreatedEvent @event           => () => When(@event),
                ProductPriceChangedEvent @event      => () => When(@event),
                ProductRenamedEvent @event           => () => When(@event),
                ProductMarkedAvailableEvent @event   => () => When(@event),
                ProductMarkedUnavailableEvent @event => () => When(@event),
                _ => null
            };
        }

        private Task When(ProductCreatedEvent @event)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            
            StoreCatalogueDbContext context = scope.ServiceProvider.GetRequiredService<StoreCatalogueDbContext>();
            if (context == null) return Task.CompletedTask;
            
            ProductDisplayEntity productDisplay = new()
            {
                Id = @event.EntityId,
                Name = @event.Name,
                Price = @event.Price,
                Description = @event.Description
            };

            context.Set<ProductDisplayEntity>().Add(productDisplay);

            return Task.CompletedTask;
        }
        
        private async Task When(ProductPriceChangedEvent @event)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            
            StoreCatalogueDbContext context = scope.ServiceProvider.GetRequiredService<StoreCatalogueDbContext>();
            if (context == null) return;
            
            DbSet<ProductDisplayEntity> set = context.Set<ProductDisplayEntity>();
            
            ProductDisplayEntity productDisplay = await set.FindAsync(@event.EntityId);
            if (productDisplay == null) return;

            productDisplay.Price = @event.NewPrice;

            set.Update(productDisplay);
        }
        
        private Task When(ProductRenamedEvent @event)
        {
            throw new NotImplementedException(); 
        }
        
        private Task When(ProductMarkedAvailableEvent @event)
        {
            throw new NotImplementedException();
        }
        
        private Task When(ProductMarkedUnavailableEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}