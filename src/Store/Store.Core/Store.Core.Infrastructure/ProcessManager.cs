using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;

namespace Store.Core.Infrastructure;

// TODO: supervisor to restart process manager if something goes bad?
public class ProcessManager<TState, TContext> : IProcessManager<TState>
    where TState : class, new()
    where TContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ProcessManager(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = Ensure.NotNull(scopeFactory);
    }
    
    public async Task HandleAsync(IProcess<TState> process, IEvent @event, EventMetadata eventMetadata)
    {
        // TODO:
        // 1. event or something comes in
        // 2. Try set to outbox (outbox doesn't do anything if command already exists)
        // 3. Save own state? (same unit of work as outbox)

        Ensure.NotNull(@event);

        using IServiceScope scope = _scopeFactory.CreateScope();
        TContext context = scope.ServiceProvider.GetRequiredService<TContext>();

        if (context == null)
            throw new InvalidOperationException(
                $"Failed to get '{typeof(TContext).FullName}' in {nameof(ProcessManager<TState, TContext>)}");

        TState currentState = await context.FindAsync<TState>(eventMetadata.CorrelationId);
        currentState ??= new();

        if (process.TryNext(currentState, @event, out TState updatedState, out object command))
        {
            context.Update(updatedState);

            if (command != null)
            {
                context.Update(new OutboxMessageEntity
                {
                    CorrelationId = eventMetadata.CorrelationId,
                    CreatedAt     = DateTime.UtcNow
                }); 
            }
        }
        
        await context.SaveChangesAsync();
    }
}