using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.Outbox;

namespace Store.Core.Infrastructure;

public class OutboxProcessor
{
    private readonly IOutbox _outbox;
    private readonly IMediator _mediator;

    public OutboxProcessor(IOutbox outbox, IMediator mediator)
    {
        _outbox   = Ensure.NotNull(outbox);
        _mediator = Ensure.NotNull(mediator);
    }
    
    public Task RunAsync() => ProcessMessages();
    
    private async Task ProcessMessages() 
    {
        IReadOnlyCollection<OutboxMessage> messages = await _outbox.GetPendingMessagesAsync();

        if (messages?.Any() != true) return;

        foreach (var message in messages)
        {
            // var command = message.ToCommand();
            await _mediator.Send(message);
            
            message.Processed();

            await _outbox.AppendAsync(message);

            // TODO 
        }
    }
}