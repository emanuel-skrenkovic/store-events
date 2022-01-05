using System.Collections.Generic;
using System.Threading.Tasks;
using Store.Core.Domain.ErrorHandling;

namespace Store.Core.Domain.Outbox;

public interface IOutbox
{
    Task<Result> AppendAsync(OutboxMessage message);

    Task<Result> SaveAsync();

    Task<IReadOnlyCollection<OutboxMessage>> GetPendingMessagesAsync();
}