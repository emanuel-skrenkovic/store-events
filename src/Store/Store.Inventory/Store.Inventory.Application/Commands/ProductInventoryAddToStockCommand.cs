using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Inventory.Application.Commands;

public record ProductInventoryAddToStockCommand(Guid ProductId, int Count) : IRequest<Result>;