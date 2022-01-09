using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Inventory.Domain;

namespace Store.Inventory.Application.Commands;

public class ProductInventoryRemoveFromStockCommandHandler : IRequestHandler<ProductInventoryRemoveFromStockCommand, Result>
{
    private readonly IAggregateRepository _repository;

    public ProductInventoryRemoveFromStockCommandHandler(IAggregateRepository repository)
        => _repository = Ensure.NotNull(repository);
    
    public Task<Result> Handle(ProductInventoryRemoveFromStockCommand request, CancellationToken cancellationToken) =>
        _repository.GetAsync<ProductInventory, Guid>(request.ProductId)
            .Then(pi => pi.RemoveFromStock(request.Count)
                .Then(() => _repository.SaveAsync<ProductInventory, Guid>(pi)));
}