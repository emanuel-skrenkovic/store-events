namespace Store.Core.Domain.Email
{
    public interface IEmailValidator
    {
        bool Validate(string emailAddress, out Error error);
    }
}