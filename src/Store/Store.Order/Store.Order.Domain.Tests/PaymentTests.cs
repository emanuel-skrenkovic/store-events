using System;
using Store.Order.Domain.Payment;
using Store.Order.Domain.Payment.Events;
using Xunit;

namespace Store.Order.Domain.Tests
{
    public class PaymentTests
    {
        private class TestPaymentNumberGenerator : IPaymentNumberGenerator
        {
            private readonly string _testValue;

            public TestPaymentNumberGenerator(string value)
            {
                _testValue = value;
            }

            public string Generate() => _testValue;
        }
        
        [Fact]
        public void Payment_Should_BeCreatedSuccessfully()
        {
            string customerNumber = Guid.NewGuid().ToString();
            string orderNumber = Guid.NewGuid().ToString();
            string paymentNumber = Guid.NewGuid().ToString();

            Guid paymentId = Guid.NewGuid();
            decimal amount = 15;

            Payment.Payment payment = Payment.Payment.Create(
                new TestPaymentNumberGenerator(paymentNumber), 
                paymentId, 
                customerNumber, 
                orderNumber, 
                amount);
            Assert.NotNull(payment);
            
            Assert.Equal(paymentId, payment.Id);
            Assert.Equal(customerNumber, payment.CustomerNumber);
            Assert.Equal(orderNumber, payment.OrderNumber);
            
            Assert.NotEmpty(payment.PaymentNumber);
            Assert.NotNull(payment.PaymentNumber);

            Assert.Equal(PaymentStatus.Approved, payment.Status);
            
            Assert.Contains(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCreatedEvent));
        }

        [Fact]
        public void Payment_ShouldComplete_WhenApproved()
        {
            string customerNumber = Guid.NewGuid().ToString();
            string orderNumber = Guid.NewGuid().ToString();
            string paymentNumber = Guid.NewGuid().ToString();

            Guid paymentId = Guid.NewGuid();
            decimal amount = 15;

            Payment.Payment payment = Payment.Payment.Create(
                new TestPaymentNumberGenerator(paymentNumber), 
                paymentId, 
                customerNumber, 
                orderNumber, 
                amount);

            payment.Complete();
            Assert.Contains(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCompletedEvent));
        }
        
        [Fact]
        public void Payment_ShouldCancel_WhenApproved()
        {
            string customerNumber = Guid.NewGuid().ToString();
            string orderNumber = Guid.NewGuid().ToString();
            string paymentNumber = Guid.NewGuid().ToString();

            Guid paymentId = Guid.NewGuid();
            decimal amount = 15;

            Payment.Payment payment = Payment.Payment.Create(
                new TestPaymentNumberGenerator(paymentNumber), 
                paymentId, 
                customerNumber, 
                orderNumber, 
                amount);

            payment.Cancel();
            Assert.Equal(PaymentStatus.Canceled, payment.Status);
            Assert.Contains(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCanceledEvent));
        }
        
        [Fact]
        public void Payment_ShouldNotComplete_WhenCanceled()
        {
            string customerNumber = Guid.NewGuid().ToString();
            string orderNumber = Guid.NewGuid().ToString();
            string paymentNumber = Guid.NewGuid().ToString();

            Guid paymentId = Guid.NewGuid();
            decimal amount = 15;

            Payment.Payment payment = Payment.Payment.Create(
                new TestPaymentNumberGenerator(paymentNumber), 
                paymentId, 
                customerNumber, 
                orderNumber, 
                amount);

            payment.Cancel();
            payment.Complete();
            
            Assert.Equal(PaymentStatus.Canceled, payment.Status);
            Assert.DoesNotContain(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCompletedEvent));
        }
        
        [Fact]
        public void Payment_ShouldNotCancel_WhenComplete()
        {
            string customerNumber = Guid.NewGuid().ToString();
            string orderNumber = Guid.NewGuid().ToString();
            string paymentNumber = Guid.NewGuid().ToString();

            Guid paymentId = Guid.NewGuid();
            decimal amount = 15;

            Payment.Payment payment = Payment.Payment.Create(
                new TestPaymentNumberGenerator(paymentNumber), 
                paymentId, 
                customerNumber, 
                orderNumber, 
                amount);

            payment.Complete();
            payment.Cancel();
            
            Assert.Equal(PaymentStatus.Completed, payment.Status);
            Assert.DoesNotContain(payment.GetUncommittedEvents(), e => e.GetType() == typeof(PaymentCanceledEvent));
        }
    }
}