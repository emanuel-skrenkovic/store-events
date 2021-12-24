using Docker.DotNet;
using Docker.DotNet.Models;
using EventStore.Client;

namespace Store.Core.Tests.Infrastructure;

public class EventStoreFixture : IDisposable
{
    private const string ConnectionString = "esdb://localhost:2111,localhost:2112,localhost:2113?tls=false&tlsVerifyCert=false";
    
    #region DockerParameters

    private string _containerId;
    
    private const string ContainerName = "store.integration-tests.eventstore";
    private const string ImageName = "eventstore/eventstore:21.6.0-buster-slim";

    // TODO: remove unnecessary
    private readonly List<string> _env = new()
    {
        "EVENTSTORE_CLUSTER_SIZE=1",
        "EVENTSTORE_RUN_PROJECTIONS=All", 
        "EVENTSTORE_START_STANDARD_PROJECTIONS=true",
        "EVENTSTORE_EXT_TCP_PORT=1113",
        "EVENTSTORE_HTTP_PORT=2113",
        "EVENTSTORE_INSECURE=true",
        "EVENTSTORE_ENABLE_EXTERNAL_TCP=true",
        "EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP=true"
    };
    
    #endregion
    
    public EventStoreClient EventStore { get; private set; }
    private readonly DockerClient _dockerClient;
    
    public EventStoreFixture()
    {
        Uri dockerUri = new(Environment.OSVersion.Platform == PlatformID.Win32NT 
            ? "npipe://./pipe/docker_engine"
            : "unix:///var/run/docker.sock");
        _dockerClient = new DockerClientConfiguration(dockerUri).CreateClient();

        EnsureRunning()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public async Task SeedAsync(Func<EventStoreClient, Task> seedAction)
    {
        await EnsureRunning();
        await seedAction(EventStore);
    }

    public async Task CleanAsync()
    {
        // TODO: Run ES in Docker container, reset container here.
        // Ugly, but ES does not support "drop database" thingy.
        await _dockerClient.Containers.RestartContainerAsync(_containerId, new ContainerRestartParameters());
    }

    public async Task<IEnumerable<object>> Events(string streamName)
    {
        EventStoreClient.ReadStreamResult eventStream = EventStore.ReadStreamAsync(
            Direction.Forwards,
            streamName, //GenerateStreamName<T, TKey>(id),
            StreamPosition.Start);

        if (await eventStream.ReadState == ReadState.StreamNotFound)
        {
            return null;
        }
            
        return await eventStream.Select(e => e.Event.Data.ToArray()).ToArrayAsync();
    }

    private async Task EnsureRunning()
    {
        var runningContainers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters());
        if (runningContainers.Any(c => c.ID == _containerId)) return;

        await StartContainer();
        await WaitUntilContainerUp();
    }
    
    #region Docker
    
    private async Task StartContainer()
    {
        CreateContainerResponse result = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = ImageName ,
            Name = ContainerName,
            // ExposedPorts = _exposedPorts,
            HostConfig = new()
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    ["1113/tcp"] = new [] { new PortBinding { HostPort = "1113" } },
                    ["2113/tcp"] = new [] { new PortBinding { HostPort = "2113" } }
                }
            },
            Env = _env
        });

        _containerId = result.ID;

        await _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters());
    }
    
    private async Task WaitUntilContainerUp()
    {
        DateTime start = DateTime.UtcNow;
        const int maxWaitTimeSeconds = 60;
        bool connectionEstablished = false;
        
        while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
        {
            try
            {
                EventStore = new EventStoreClient(EventStoreClientSettings.Create(ConnectionString));
                await EventStore.ReadAllAsync(Direction.Backwards, Position.End).FirstAsync();
                
                connectionEstablished = true;
            }
            catch
            {
                await Task.Delay(100);
            }
        }

        if (!connectionEstablished)
        {
            throw new Exception($"Connection to EventStore could not be established within {maxWaitTimeSeconds} seconds.");
        }
    }

    #endregion
    
    #region IDisposable
    
    private void ReleaseUnmanagedResources()
    {
        EventStore.Dispose();

        _dockerClient.Containers
            .StopContainerAsync(_containerId, new ContainerStopParameters())
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        _dockerClient.Containers
            .RemoveContainerAsync(_containerId, new ContainerRemoveParameters())
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~EventStoreFixture()
    {
        ReleaseUnmanagedResources();
    }
    
    #endregion
}