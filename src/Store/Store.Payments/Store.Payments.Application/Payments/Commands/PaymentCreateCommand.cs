using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Payments.Application.Payments.Commands;

public record PaymentCreateCommand(
    Guid OrderId, 
    string Source, 
    decimal Amount, 
    string Note = null) : IRequest<Result<PaymentCreateResponse>>;