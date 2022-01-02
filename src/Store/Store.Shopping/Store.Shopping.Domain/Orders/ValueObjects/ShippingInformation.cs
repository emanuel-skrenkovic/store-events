using System.Collections.Generic;
using System.Linq;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Domain.Orders.ValueObjects;

public record ShippingInfo(
    int CountryCode,
    string FullName,
    string StreetAddress,
    string City,
    string StateProvince,
    string Postcode,
    string PhoneNumber);

// TODO: what about GDPR?
public class ShippingInformation : ValueObject<ShippingInformation>
{
    public int CountryCode { get; } // TODO
    
    public string FullName { get; }
    
    public string StreetAddress { get; }
        
    public string City { get; }
        
    public string StateProvince { get; }
        
    public string Postcode { get; }
        
    public string PhoneNumber { get; }
    
    public ShippingInformation(
        int    countryCode,
        string fullName,
        string streetAddress,
        string city,
        string stateProvince,
        string postcode,
        string phoneNumber)
    {
        CountryCode = countryCode;
        FullName = fullName;
        StreetAddress = streetAddress;
        City = city;
        StateProvince = stateProvince;
        Postcode = postcode;
        PhoneNumber = phoneNumber;
    }

    public static Result<ShippingInformation> Create(
        int    countryCode,
        string fullName,
        string streetAddress,
        string city,
        string stateProvince,
        string postcode,
        string phoneNumber)
    {
        List<Error> errors = new();
        
        // TODO: countryCode
        if (string.IsNullOrWhiteSpace(fullName))      errors.Add(new($"{fullName} is null or empty."));
        if (string.IsNullOrWhiteSpace(streetAddress)) errors.Add(new($"{streetAddress} is null or empty."));
        if (string.IsNullOrWhiteSpace(city))          errors.Add(new($"{city} is null or empty."));
        if (string.IsNullOrWhiteSpace(stateProvince)) errors.Add(new($"{stateProvince} is null or empty."));
        if (string.IsNullOrWhiteSpace(postcode))      errors.Add(new($"{postcode} is null or empty."));
        if (string.IsNullOrWhiteSpace(phoneNumber))   errors.Add(new($"{phoneNumber} is null or empty."));

        if (errors.Any())
        {
            return new ValidationError(
                $"Validation failed for '{nameof(ShippingInformation)}'.",
                errors.ToArray()); 
        }

        return new ShippingInformation(
            countryCode, 
            fullName, 
            streetAddress, 
            city,
            stateProvince, 
            postcode, 
            phoneNumber);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return FullName;
        yield return StreetAddress;
        yield return City;
        yield return StateProvince;
        yield return Postcode;
        yield return PhoneNumber;
    }
    
    internal ShippingInfo ToInfo() => new(CountryCode, FullName, StreetAddress, City, StateProvince, Postcode, PhoneNumber);

    internal static ShippingInformation FromValues(ShippingInfo info) => new(
        info.CountryCode, info.FullName, info.StreetAddress, info.City, info.StateProvince, info.Postcode, info.PhoneNumber);
}