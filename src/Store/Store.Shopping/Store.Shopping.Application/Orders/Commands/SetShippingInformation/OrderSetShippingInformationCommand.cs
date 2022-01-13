using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Application.Orders.Commands.SetShippingInformation;

public record OrderSetShippingInformationCommand(Guid OrderId, ShippingInfo ShippingInfo) : IRequest<Result>;