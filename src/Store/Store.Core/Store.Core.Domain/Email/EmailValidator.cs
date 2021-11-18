using System;
using System.Net.Mail;

namespace Store.Core.Domain.Email
{
    public class EmailValidator : IEmailValidator
    {
        public bool Validate(string emailAddress, out Error error)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(emailAddress);

                if (emailAddress != mailAddress.Address)
                {
                    error = new("Provided email address string does not match the parsed email address.");
                    return false;
                }

                error = null;
                return true;
            }
            catch (FormatException formatException)
            {
                error = new(formatException.Message);
                return false;
            }
        }
    }
}