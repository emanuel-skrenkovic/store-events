using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Orders;

namespace Store.Shopping.Application.Orders.Commands.AddShippingInformation;

public record OrderAddShippingInformationCommand(Guid OrderId, ShippingInformation ShippingInformation) : IRequest<Result>;