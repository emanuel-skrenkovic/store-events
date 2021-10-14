using System.ComponentModel.DataAnnotations;

namespace Store.Catalogue.AspNet.Models.Product
{
    public class ProductPostApiModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        public string Description { get; set; }
    }
}