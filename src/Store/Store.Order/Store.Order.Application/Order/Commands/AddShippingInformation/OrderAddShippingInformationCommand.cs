using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Domain.Orders;

namespace Store.Order.Application.Order.Commands.AddShippingInformation;

public record OrderAddShippingInformationCommand(Guid OrderId, ShippingInformation ShippingInformation) : IRequest<Result>;