using Microsoft.EntityFrameworkCore;

namespace Store.Core.Tests.Infrastructure;

public class PostgresFixture<TContext> : IDisposable where TContext : DbContext
{
    // TODO: really don't like this. Improve.
    private const string ConnectionString = 
        "User ID=postgres;Password=postgres;Server=localhost;Port=5432;Database=store-shopping;Integrated Security=true;Pooling=true;";
    
    #region DockerParameters

    private const string ContainerName = "store.integration-tests.postgres";
    private const string ImageName = "postgres:14-alpine";

    private readonly List<string> _env = new()
    {
        "POSTGRES_USER=postgres",
        "POSTGRES_PASSWORD=postgres"
    };

    private readonly Dictionary<string, string> _ports = new()
    {
        ["5432"] = "5432"
    };
    
    #endregion
    
    private readonly DockerContainer _container;
    
    public TContext Context { get; private set; }

    public PostgresFixture()
    {
        _container = new(ContainerName, ImageName, _env, _ports);
        _container.EnsureRunningAsync(CheckConnectionAsync)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
        
        EnsureMigratedAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public async Task SeedAsync(Func<TContext, Task> seedAction)
    {
        await EnsureMigratedAsync();
        await seedAction(Context);
    }

    public async Task CleanAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await EnsureMigratedAsync();
    }

    private Task EnsureMigratedAsync() => Context.Database.MigrateAsync();

    private async Task<bool> CheckConnectionAsync()
    {
        try
        {
            DbContextOptionsBuilder<TContext> optionsBuilder = new();
            optionsBuilder.UseNpgsql(ConnectionString);
            
            Context = (TContext)Activator.CreateInstance(
                typeof(TContext),
                optionsBuilder.Options);
            if (Context == null) return false;
            await EnsureMigratedAsync();

            return true;
        }
        catch
        {
            return false; 
        }
    }

    #region IDisposable

    private void ReleaseUnmanagedResources()
    {
        Context.Dispose();
        _container.Dispose();
    }

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