namespace Store.Order.Domain.Orders
{
    public record OrderLine(Item Item, uint Count);
}