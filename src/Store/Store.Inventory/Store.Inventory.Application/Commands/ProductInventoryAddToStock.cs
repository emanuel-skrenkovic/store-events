using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Inventory.Domain;
using Store.Inventory.Domain.ValueObjects;

namespace Store.Inventory.Application.Commands;

public record ProductInventoryAddToStockCommand(Guid ProductId, int Count) : IRequest<Result>;

public class ProductInventoryAddToStock : IRequestHandler<ProductInventoryAddToStockCommand, Result>
{
    private readonly IAggregateRepository _repository;

    public ProductInventoryAddToStock(IAggregateRepository repository)
    {
        _repository = Ensure.NotNull(repository);
    }
    
    public async Task<Result> Handle(ProductInventoryAddToStockCommand request, CancellationToken cancellationToken)
    {
        (Guid productId, int count) = request;
        Result<ProductInventory> getProductInventoryResult = await _repository.GetAsync<ProductInventory, Guid>(productId);
        
        ProductInventory productInventory = getProductInventoryResult
            .UnwrapOrDefault(ProductInventory.Create(new ProductNumber(productId)));
        
        productInventory.AddToStock(count);

        return await _repository.SaveAsync<ProductInventory, Guid>(productInventory);
    }
}