using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Orders.Commands.CreateOrder;

public record OrderCreateCommand(string CustomerNumber, string SessionId) : IRequest<Result<OrderCreateResponse>>;