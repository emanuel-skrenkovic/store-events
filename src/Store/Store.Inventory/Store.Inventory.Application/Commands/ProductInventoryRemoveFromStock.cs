using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Inventory.Domain;

namespace Store.Inventory.Application.Commands;

public record ProductInventoryRemoveFromStockCommand(Guid ProductId, int Count) : IRequest<Result>;

public class ProductInventoryRemoveFromStock : IRequestHandler<ProductInventoryRemoveFromStockCommand, Result>
{
    private readonly IAggregateRepository _repository;

    public ProductInventoryRemoveFromStock(IAggregateRepository repository)
        => _repository = Ensure.NotNull(repository);
    
    public Task<Result> Handle(ProductInventoryRemoveFromStockCommand request, CancellationToken ct) =>
        _repository
            .GetAsync<ProductInventory, Guid>(request.ProductId, ct)
            .Then
            (
                pi => pi
                    .RemoveFromStock(request.Count)
                    .Then(() => _repository.SaveAsync<ProductInventory, Guid>(pi, ct))
            );
}