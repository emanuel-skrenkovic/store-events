namespace Store.Order.Infrastructure.Entity
{
    public class OrderEntity
    {
        public Guid OrderId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string CustomerNumber { get; set; }
        // TODO: should I put session id here as well?

        public string Data { get; set; }
    } 
}
