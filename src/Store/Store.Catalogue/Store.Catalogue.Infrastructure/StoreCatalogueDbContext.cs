using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Infrastructure;

public class StoreCatalogueDbContext : DbContext
{
    private readonly IIntegrationEventMapper _eventMapper;
    private readonly IEventDispatcher        _eventDispatcher;
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DbSet<ProductEntity> Products { get; set; }

    public StoreCatalogueDbContext(
        DbContextOptions<StoreCatalogueDbContext> options,
        IIntegrationEventMapper eventMapper,
        IEventDispatcher eventDispatcher) : base(options)
    {
        _eventMapper     = eventMapper     ?? throw new ArgumentNullException(nameof(eventMapper));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
    }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
            
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreCatalogueDbContext).Assembly);
            
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        List<object> integrationEvents = new();

        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (_eventMapper.TryMap(entry, out IEvent integrationEvent))
            {
                integrationEvents.Add(integrationEvent);
            }
        }

        int result = await base.SaveChangesAsync(cancellationToken);
        
        if (result > 0 && integrationEvents.Any())
        {
            await Task.WhenAll(integrationEvents.Select(e => _eventDispatcher.DispatchAsync(e)));
        }

        return result;
    }
}