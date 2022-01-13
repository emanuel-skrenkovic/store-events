using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Orders.Commands.Confirm;

public record OrderConfirmCommand(Guid OrderId) : IRequest<Result>;