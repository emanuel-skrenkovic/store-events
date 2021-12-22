namespace Store.Shopping.Infrastructure.Entity;

public class ShippingInformation
{
    public int CountryCode { get; set; }
        
    public string FullName { get; set; }
        
    public string StreetAddress { get; set; }
        
    public string City { get; set; }
        
    public string StateProvince { get; set; }
        
    public string Postcode { get; set; }
        
    public string PhoneNumber { get; set; }
}