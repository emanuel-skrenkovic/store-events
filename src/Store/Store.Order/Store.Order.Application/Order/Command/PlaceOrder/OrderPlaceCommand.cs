using MediatR;
using Store.Core.Domain.Result;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Order.Command.PlaceOrder
{
    public record OrderPlaceCommand(string CustomerNumber) : IRequest<Result<Unit>>;
}