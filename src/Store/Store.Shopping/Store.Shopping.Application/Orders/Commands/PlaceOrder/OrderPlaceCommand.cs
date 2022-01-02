using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Orders.Commands.PlaceOrder;

public record OrderPlaceCommand(
    string CustomerNumber, 
    string SessionId,
    Guid PaymentNumber,
    ShippingInfo ShippingInfo) : IRequest<Result<OrderPlaceResponse>>;