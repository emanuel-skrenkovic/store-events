using System;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Orders.ValueObjects;
using Store.Shopping.Domain.Payments.ValueObjects;
using Store.Shopping.Domain.ValueObjects;

namespace Store.Shopping.Domain;

public class OrderPaymentService : IOrderPaymentService
{
    public Result<Payments.Payment> CreateOrderPayment(Orders.Order order)
    {
        Ensure.NotNull(order, nameof(order));

        return Payments.Payment.Create(
            new PaymentNumber(Guid.NewGuid()),
            new CustomerNumber(order.CustomerNumber), 
            new OrderNumber(order.Id),
            order.Total); // TODO
    }
}