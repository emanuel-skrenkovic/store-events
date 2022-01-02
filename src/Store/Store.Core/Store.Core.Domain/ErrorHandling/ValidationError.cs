namespace Store.Core.Domain.ErrorHandling;

public record ValidationError(string Message, Error[] Errors) : Error(Message);