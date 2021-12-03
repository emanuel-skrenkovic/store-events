using System;

namespace Store.Order.Infrastructure.Entity
{
    public class CartEntryEntity
    {
        public string CustomerNumber { get; set; }
        
        public string SessionId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public uint Quantity { get; set; }

        public string ProductCatalogueNumber { get; set; }
        
        public ProductEntity Product { get; set; }
    }
}