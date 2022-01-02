namespace Store.Shopping.Application.Orders;

public record ShippingInfo(
    int    CountryCode,
    string FullName,
    string StreetAddress,
    string City,
    string StateProvince,
    string Postcode,
    string PhoneNumber);