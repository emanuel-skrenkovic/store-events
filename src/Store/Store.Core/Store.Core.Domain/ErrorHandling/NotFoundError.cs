namespace Store.Core.Domain.ErrorHandling
{
    public record NotFoundError(string Message) : Error(Message);
}