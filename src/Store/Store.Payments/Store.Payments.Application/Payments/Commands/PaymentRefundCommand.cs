using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Payments.Application.Payments.Commands;

public record PaymentRefundCommand(Guid PaymentId, string Note = null) : IRequest<Result<PaymentRefundResponse>>;