using System;

namespace Store.Order.Infrastructure.Entity
{
    public class OrderLineEntity
    {
        public uint Id { get; set; }
        
        public ProductEntity Product { get; set; }
        
        public string ProductCatalogueNumber { get; set; }
        
        public uint Count { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public Guid OrderId { get; set; }
        
        public OrderEntity OrderEntity { get; set; }
    }
}