using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Payments.Commands.Complete;

public record PaymentCompleteCommand(Guid PaymentId) : IRequest<Result>;