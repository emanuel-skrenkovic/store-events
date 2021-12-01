using System;
using MediatR;
using Store.Core.Domain.ErrorHandling;

namespace Store.Order.Application.Payment.Commands.Complete
{
    public record PaymentCompleteCommand(Guid PaymentId) : IRequest<Result>;
}