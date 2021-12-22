using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Orders.Commands.CreatePayment;

public record CreateOrderPaymentCommand(Guid OrderId) : IRequest<Result>;