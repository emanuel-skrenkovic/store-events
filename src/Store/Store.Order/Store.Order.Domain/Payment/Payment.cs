using System;
using Store.Core.Domain;
using Store.Core.Domain.Functional;
using Store.Core.Domain.Result;
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

        public static Payment Create(
            IPaymentNumberGenerator paymentNumberGenerator, 
            Guid id, 
            string customerNumber, 
            string orderNumber,
            decimal amount)
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
                amount,
                PaymentStatus.Approved));

            return payment;
        }

        private void Apply(PaymentCreatedEvent domainEvent)
        {
            Id = domainEvent.EntityId;
            PaymentNumber = domainEvent.PaymentNumber;
            CustomerNumber = domainEvent.CustomerNumber;
            OrderNumber = domainEvent.OrderNumber;
            Amount = domainEvent.Amount;
            Status = domainEvent.Status;
        }

        public Result<Unit> Complete()
        {
            if (Status != PaymentStatus.Approved) return new Error("Cannot complete unapproved payment.");
            
            ApplyEvent(new PaymentCompletedEvent(Id));
            return Unit.Value;
        }

        private void Apply(PaymentCompletedEvent _)
        {
            Status = PaymentStatus.Completed;
        }

        public Result<Unit> Cancel()
        {
            if (Status == PaymentStatus.Completed) return new Error("Payment was already completed.");
            
            ApplyEvent(new PaymentCanceledEvent(Id));
            return Unit.Value;
        }

        private void Apply(PaymentCanceledEvent _)
        {
            Status = PaymentStatus.Canceled;
        }
        
        #region Wiring
        
        protected override void RegisterAppliers()
        {
            RegisterApplier<PaymentCreatedEvent>(Apply);
            RegisterApplier<PaymentCompletedEvent>(Apply);
            RegisterApplier<PaymentCanceledEvent>(Apply);
        }
        
        #endregion
    }
}