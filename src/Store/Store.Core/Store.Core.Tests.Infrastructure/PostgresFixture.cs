using Microsoft.EntityFrameworkCore;

namespace Store.Core.Tests.Infrastructure;

public class PostgresFixture<TContext> : IDisposable where TContext : DbContext
{
    // TODO: really don't like this. Improve.
    private const string ConnectionString = 
        "User ID=postgres;Password=postgres;Server=localhost;Port=5432;Database=store-shopping;Integrated Security=true;Pooling=true;";
    
    private readonly TContext _context;

    public PostgresFixture()
    {
        DbContextOptionsBuilder<TContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(ConnectionString);
        
        _context = (TContext) Activator.CreateInstance(
            typeof(TContext), 
            optionsBuilder.Options);
    }

    public async Task SeedAsync(Func<TContext, Task> seedAction)
    {
        await EnsureDatabaseAsync();
        await seedAction(_context);
    }

    public async Task CleanAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await EnsureDatabaseAsync();
    }

    private Task EnsureDatabaseAsync() => _context.Database.MigrateAsync();

    #region IDisposable
    
    private void ReleaseUnmanagedResources() => _context.Dispose();

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~PostgresFixture()
    {
        ReleaseUnmanagedResources();
    }
    
    #endregion
}