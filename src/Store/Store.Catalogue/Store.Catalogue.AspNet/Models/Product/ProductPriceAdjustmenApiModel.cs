namespace Store.Catalogue.AspNet.Models.Product
{
    public class ProductPriceAdjustmentApiModel
    {
        public decimal NewPrice { get; set; }
        
        public string Reason { get; set; }
    }
}