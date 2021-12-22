namespace Store.Shopping.Domain.Orders;

public record ShippingInformation(
    int CountryCode, // TODO
    string FullName,
    string StreetAddress,
    string City,
    string StateProvince,
    string Postcode,
    string PhoneNumber);