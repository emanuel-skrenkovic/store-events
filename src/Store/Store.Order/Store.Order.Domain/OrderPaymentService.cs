using System;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Domain.Orders.ValueObjects;
using Store.Order.Domain.Payment.ValueObjects;
using Store.Order.Domain.ValueObjects;

namespace Store.Order.Domain
{
    public class OrderPaymentService : IOrderPaymentService
    {
        public Result<Payment.Payment> CreateOrderPayment(Orders.Order order)
        {
            Ensure.NotNull(order, nameof(order));

            return Payment.Payment.Create(
                new PaymentNumber(Guid.NewGuid()),
                new CustomerNumber(order.CustomerNumber), 
                new OrderNumber(order.Id),
                order.Total); // TODO
        }
    }
}