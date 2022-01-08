using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Store.Core.Domain;

namespace Store.Core.Infrastructure;

public class OutboxProcessor : IJob
{
    private readonly ISerializer _serializer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDbConnection _db;
    private readonly IMediator _mediator;

    public OutboxProcessor(ISerializer serializer, IServiceScopeFactory scopeFactory, IDbConnection db, IMediator mediator)
    {
        _serializer   = Ensure.NotNull(serializer);
        _scopeFactory = Ensure.NotNull(scopeFactory);
        _db           = Ensure.NotNull(db);
        _mediator     = Ensure.NotNull(mediator);
    }
    
    private async Task ProcessMessages()
    {
        const string unprocessedMessagesQuery =
            @"SELECT * FROM public.outbox_message
              WHERE processed_at IS NULL;";
        
        var messages = (await _db.QueryAsync<OutboxMessageEntity>(unprocessedMessagesQuery))
            ?.ToList();
        if (messages?.Any() != true) return;

        foreach (OutboxMessageEntity message in messages)
        {
            // Create scope to isolate CorrelationContext.
            using IServiceScope _ = _scopeFactory.CreateScope();
            
            CorrelationContext.SetMessageId(message.Id);
            CorrelationContext.SetCorrelationId(message.CorrelationId);
            CorrelationContext.SetCausationId(message.Id);
            
            var command = _serializer.Deserialize(message.Data, Type.GetType(message.Type));
            await _mediator.Send(command); // TODO: how do I know this worked? I don't. That's why I retry.
            
            const string updateCommand =
                @"UPDATE public.outbox_message
                  SET processed_at = @processedAt
                  WHERE message_id = @id;";

            await _db.ExecuteAsync(
                updateCommand, 
                new { id = message.Id, processedAt = DateTime.UtcNow });
        }
    }
    
    #region IJob

    public Task Execute(IJobExecutionContext context) => ProcessMessages();
    
    #endregion
}