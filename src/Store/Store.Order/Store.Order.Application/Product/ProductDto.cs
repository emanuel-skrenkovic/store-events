namespace Store.Order.Application.Product
{
    public class ProductDto
    {
        public string CatalogueNumber { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public decimal Price { get; set; }
    }
}