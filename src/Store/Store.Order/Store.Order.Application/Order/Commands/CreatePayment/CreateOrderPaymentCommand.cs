using System;
using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Order.Application.Order.Commands.CreatePayment
{
    public record CreateOrderPaymentCommand(Guid OrderId) : IRequest<Result>;
}