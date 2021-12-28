using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EventStore.Extensions;
using Xunit;

namespace Store.Core.Tests.Infrastructure;

public class EventStoreFixture : IAsyncLifetime
{
    private const string ConnectionString = "esdb://localhost:2111,localhost:2112,localhost:2113?tls=false&tlsVerifyCert=false";
    
    #region DockerParameters
    
    private const string ContainerName = "store.integration-tests.eventstore";
    private const string ImageName = "eventstore/eventstore:21.6.0-buster-slim";

    // TODO: remove unnecessary
    private readonly List<string> _env = new()
    {
        "EVENTSTORE_CLUSTER_SIZE=1",
        "EVENTSTORE_EXT_TCP_PORT=1113",
        "EVENTSTORE_HTTP_PORT=2113",
        "EVENTSTORE_INSECURE=true",
        "EVENTSTORE_ENABLE_EXTERNAL_TCP=true",
        "EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP=true"
    };

    private readonly Dictionary<string, string> _ports = new()
    {
        ["1113"] = "1113",
        ["2113"] = "2113"
    };
    
    #endregion
    
    public EventStoreClient EventStore { get; private set; }
    private readonly DockerContainer _container;
    
    public EventStoreFixture()
    {
        _container = new(ContainerName, ImageName, _env, _ports);
        _container.CheckStatus = CheckConnectionAsync;
    }

    public async Task SeedAsync(Func<EventStoreClient, Task> seedAction)
    {
        await _container.InitializeAsync();
        await seedAction(EventStore);
    }

    public async Task CleanAsync()
    {
        // TODO: Run ES in Docker container, reset container here.
        // Ugly, but ES does not support "drop database" thingy.
        await _container.RestartAsync(CheckConnectionAsync);
    }

    public async Task<List<IEvent>> Events(string streamName)
    {
        EventStoreClient.ReadStreamResult eventStream = EventStore.ReadStreamAsync(
            Direction.Forwards,
            streamName, //GenerateStreamName<T, TKey>(id),
            StreamPosition.Start);

        if (await eventStream.ReadState == ReadState.StreamNotFound)
        {
            return null;
        }

        ISerializer serializer = new JsonSerializer();
        return await eventStream
            .Select(e => e.Deserialize(serializer) as IEvent)
            .ToListAsync();
    }

    private async Task<bool> CheckConnectionAsync()
    {
        try
        {
            EventStore = new EventStoreClient(EventStoreClientSettings.Create(ConnectionString));
            await EventStore.ReadAllAsync(Direction.Backwards, Position.End).FirstAsync();

            return true;
        }
        catch
        {
            return false;
        } 
    }
    
    #region IAsyncLifetime

    public Task InitializeAsync()
    {
        return _container.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await EventStore.DisposeAsync();
        await _container.DisposeAsync();
    }
    
    #endregion
}