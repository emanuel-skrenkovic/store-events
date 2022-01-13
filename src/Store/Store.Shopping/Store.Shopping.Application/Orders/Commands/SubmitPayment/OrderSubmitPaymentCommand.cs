using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Orders.Commands.SubmitPayment;

public record OrderSubmitPaymentCommand(Guid OrderId, Guid PaymentId) : IRequest<Result>;