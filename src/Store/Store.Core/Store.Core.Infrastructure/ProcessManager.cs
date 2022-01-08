using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.ProcessManager;
using Store.Core.Infrastructure.EntityFramework.Extensions;

namespace Store.Core.Infrastructure;

// TODO: supervisor to restart process manager if something goes bad?
public class ProcessManager<TState, TContext> : IProcessManager<TState>, IEventListener
    where TState : class, new()
    where TContext : DbContext
{
    private readonly ISerializer _serializer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventSubscriptionFactory _eventSubscriptionFactory;
    private readonly IProcess<TState> _process;

    private string SubscriptionId => _process.GetType().Name;

    public ProcessManager(
        ISerializer serializer, 
        IServiceScopeFactory scopeFactory, 
        IEventSubscriptionFactory eventSubscriptionFactory,
        IProcess<TState> process)
    {
        _serializer = Ensure.NotNull(serializer);
        _scopeFactory = Ensure.NotNull(scopeFactory);
        _eventSubscriptionFactory = Ensure.NotNull(eventSubscriptionFactory);
        _process = Ensure.NotNull(process);
    }

    public async Task HandleAsync(IEvent @event, EventMetadata eventMetadata)
    {
        Ensure.NotNull(@event);
        Ensure.NotNull(eventMetadata);

        using IServiceScope scope = _scopeFactory.CreateScope();
        TContext context = scope.ServiceProvider.GetRequiredService<TContext>();

        if (context == null)
            throw new InvalidOperationException(
                $"Failed to get '{typeof(TContext).FullName}' in {nameof(ProcessManager<TState, TContext>)}");

        ProcessEntity processEntity = await context.FindAsync<ProcessEntity>(eventMetadata.CorrelationId);

        TState currentState;
        if (processEntity == null)
        {
            processEntity = new();
            currentState = new();

            context.Attach(processEntity);
        }
        else
        {
            currentState = _serializer.Deserialize(
                processEntity.Data, 
                Type.GetType(processEntity.Type)) as TState;             
        }

        if (!_process.TryNext(currentState, @event, out TState updatedState, out object command))
            return;

        processEntity.Data = _serializer.Serialize(updatedState);

        if (command != null)
        {
            context.Add(new OutboxMessageEntity
            {
                Id            = CorrelationContext.MessageId,
                CorrelationId = eventMetadata.CorrelationId,
                CausationId   = eventMetadata.CausationId,
                CreatedAt     = DateTime.UtcNow,
                Type          = command.GetType().FullName,
                Data          = _serializer.Serialize(command)
            }); 
        }
        
        await context.AddOrUpdateSubscriptionCheckpoint(SubscriptionId, eventMetadata.StreamPosition);
        await context.SaveChangesAsync();
    }
    
    #region IEventListener

    public async Task StartAsync()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        if (context == null)
        {
            throw new InvalidOperationException($"Context cannot be null on {nameof(ProcessManager<TState, TContext>)} startup.");
        }
            
        ulong checkpoint = await context.GetSubscriptionCheckpoint(SubscriptionId);
            
        await _eventSubscriptionFactory
            .Create(SubscriptionId, HandleAsync)
            .SubscribeAtAsync(checkpoint);
    }

    public Task StopAsync() => Task.CompletedTask;

    #endregion
}