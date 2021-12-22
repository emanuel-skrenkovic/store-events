using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Payments.Commands.Cancel;

public record PaymentCancelCommand(Guid PaymentId) : IRequest<Result>;