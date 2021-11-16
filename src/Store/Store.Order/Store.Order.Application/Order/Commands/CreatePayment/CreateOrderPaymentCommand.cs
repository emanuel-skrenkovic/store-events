using System;
using MediatR;
using Store.Core.Domain.Result;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Order.Commands.CreatePayment
{
    public record CreateOrderPaymentCommand(Guid OrderNumber) : IRequest<Result<Unit>>;
}