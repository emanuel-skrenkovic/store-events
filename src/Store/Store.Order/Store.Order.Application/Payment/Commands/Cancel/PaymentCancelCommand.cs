using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Order.Application.Payment.Commands.Cancel;

public record PaymentCancelCommand(Guid PaymentId) : IRequest<Result>;