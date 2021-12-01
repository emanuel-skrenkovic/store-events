using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Order.Application.Order.Commands.PlaceOrder
{
    public record OrderPlaceCommand(string CustomerNumber, string SessionId) : IRequest<Result>;
}