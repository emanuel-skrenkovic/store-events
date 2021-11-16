using System;
using Store.Core.Domain;
using Store.Core.Domain.Result;
using Store.Order.Domain.Payment;

namespace Store.Order.Domain
{
    public class OrderPaymentService : IOrderPaymentService
    {
        private readonly IPaymentNumberGenerator _paymentNumberGenerator;

        public OrderPaymentService(IPaymentNumberGenerator paymentNumberGenerator)
        {
            _paymentNumberGenerator = paymentNumberGenerator ?? throw new ArgumentNullException(nameof(paymentNumberGenerator));
        }
        
        public Result<Payment.Payment> CreateOrderPayment(Orders.Order order)
        {
            Ensure.NotNull(order, nameof(order));

            return Payment.Payment.Create(
                _paymentNumberGenerator, 
                Guid.NewGuid(), 
                order.CustomerNumber, 
                order.Id.ToString(), 
                order.Amount); // TODO
        }
    }
}