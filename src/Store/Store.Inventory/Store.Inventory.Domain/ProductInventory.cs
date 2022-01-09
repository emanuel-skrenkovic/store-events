using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Inventory.Domain.Events;
using Store.Inventory.Domain.ValueObjects;

namespace Store.Inventory.Domain;

public class ProductInventory : AggregateEntity<Guid>
{
    public int Count { get; private set; }
    
    public static ProductInventory Create(ProductNumber productNumber)
    {
        ProductInventory productInventory = new();
        productInventory.ApplyEvent(new ProductInventoryCreatedEvent(productNumber.Value));

        return productInventory;
    }

    private void Apply(ProductInventoryCreatedEvent domainEvent)
    {
        Id = domainEvent.ProductId;
        Count = 0;
    }

    public void AddToStock(int count)
    {
        ApplyEvent(new ProductInventoryAddedToStockEvent(Id, count));
    }

    private void Apply(ProductInventoryAddedToStockEvent domainEvent)
    {
        Count += domainEvent.Count;
    }

    public Result RemoveFromStock(int count)
    {
        if (count > Count) 
            return new Error($"Not enough items in stock. Tried to remove '{count}', currently available '{Count}'.");
        
        ApplyEvent(new ProductInventoryRemovedFromStockEvent(Id, count));

        return Result.Ok();
    }

    private void Apply(ProductInventoryRemovedFromStockEvent domainEvent)
    {
        Count -= domainEvent.Count;
    }
    
    protected override void RegisterAppliers()
    {
        RegisterApplier<ProductInventoryCreatedEvent>(Apply);
        RegisterApplier<ProductInventoryAddedToStockEvent>(Apply);
        RegisterApplier<ProductInventoryRemovedFromStockEvent>(Apply);
    }
}