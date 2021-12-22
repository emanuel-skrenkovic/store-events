using System;
using System.Collections.Generic;
using Store.Core.Domain;
using Store.Core.Domain.Email;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Domain.ValueObjects;

public class EmailAddress : ValueObject<EmailAddress>
{
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static EmailAddress FromString(string address, IEmailValidator emailValidator)
    {
        if (!emailValidator.Validate(address, out Error error))
        {
            throw new ArgumentException(error.Message);
        }
            
        return new(address);
    }
        
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}