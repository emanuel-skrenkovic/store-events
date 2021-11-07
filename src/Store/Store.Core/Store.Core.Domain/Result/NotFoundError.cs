namespace Store.Core.Domain.Result
{
    public record NotFoundError(string Message) : Error(Message);
}