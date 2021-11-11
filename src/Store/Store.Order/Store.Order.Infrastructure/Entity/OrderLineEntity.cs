namespace Store.Order.Infrastructure.Entity
{
    public class OrderLineEntity
    {
        public string ItemCatalogueNumber { get; set; }
        
        public uint Count { get; set; }
    }
}