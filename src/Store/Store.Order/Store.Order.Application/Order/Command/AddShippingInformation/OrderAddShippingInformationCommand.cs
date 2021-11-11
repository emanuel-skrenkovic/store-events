using System;
using MediatR;
using Store.Core.Domain.Result;
using Store.Order.Domain.Orders;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Order.Command.AddShippingInformation
{
    public record OrderAddShippingInformationCommand(Guid OrderNumber, ShippingInformation ShippingInformation) : IRequest<Result<Unit>>;
}