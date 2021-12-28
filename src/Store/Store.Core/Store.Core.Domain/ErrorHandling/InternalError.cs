namespace Store.Core.Domain.ErrorHandling;

public record InternalError(string Message, string StackTrace) : Error(Message);