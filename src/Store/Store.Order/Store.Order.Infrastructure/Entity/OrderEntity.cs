using System;
using System.Collections.Generic;

namespace Store.Order.Infrastructure.Entity
{
    public class OrderEntity
    {
        public Guid OrderId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string CustomerNumber { get; set; }
        
        // TODO: should I put session id here as well?
        
        public decimal TotalAmount { get; set; }
        
        public ICollection<OrderLineEntity> OrderLines { get; set; }
        
        public ShippingInformationEntity ShippingInformation { get; set; }
    } 
}
