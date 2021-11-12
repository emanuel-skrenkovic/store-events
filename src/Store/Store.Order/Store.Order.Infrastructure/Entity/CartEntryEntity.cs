using System;

namespace Store.Order.Infrastructure.Entity
{
    public class CartEntryEntity
    {
        public Guid Id { get; set; }

        public string CatalogueNumber { get; set; }
        
        public uint Quantity { get; set; }
    }
}