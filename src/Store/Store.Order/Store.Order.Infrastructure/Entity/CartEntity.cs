namespace Store.Order.Infrastructure.Entity
{
    public class CartEntity
    {
        public int Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string CustomerNumber { get; set; }
        
        public string SessionId { get; set; }
        
        public string Data { get; set; }
    }
}