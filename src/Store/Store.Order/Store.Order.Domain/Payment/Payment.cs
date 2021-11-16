using System;
using Store.Core.Domain;
using Store.Order.Domain.Payment.Events;

namespace Store.Order.Domain.Payment
{
    public class Payment : AggregateEntity<Guid>
    {
        public string PaymentNumber { get; private set; }
        
        public string OrderNumber { get; private set; }
        
        public string CustomerNumber { get; private set; }
        
        public PaymentStatus Status { get; private set; }
        
        public BillingAddress BillingAddress { get; private set; }
        
        public decimal Amount { get; private set; }

        public static Payment Create(IPaymentNumberGenerator paymentNumberGenerator, Guid id, string customerNumber, string orderNumber)
        {
            // TODO: think about returning Error.
            Ensure.NotNull(paymentNumberGenerator, nameof(paymentNumberGenerator));
            Ensure.NotNullOrEmpty(customerNumber, Ensure.CommonMessages.NullOrEmpty(nameof(customerNumber)));
            Ensure.NotNullOrEmpty(orderNumber, Ensure.CommonMessages.NullOrEmpty(nameof(orderNumber)));
            
            Payment payment = new();
            payment.ApplyEvent(new PaymentCreatedEvent(
                id, 
                paymentNumberGenerator.Generate(), 
                customerNumber, 
                orderNumber, 
                PaymentStatus.Pending));

            return payment;
        }

        private void Apply(PaymentCreatedEvent domainEvent)
        {
            Id = domainEvent.EntityId;
            PaymentNumber = domainEvent.PaymentNumber;
            CustomerNumber = domainEvent.CustomerNumber;
            OrderNumber = domainEvent.OrderNumber;
            Status = domainEvent.Status;
        }
        
        protected override void RegisterAppliers()
        {
            RegisterApplier<PaymentCreatedEvent>(Apply);
        }
    }
}