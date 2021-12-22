using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Orders.Commands.PlaceOrder;

public record OrderPlaceCommand(string CustomerNumber, string SessionId) : IRequest<Result>;