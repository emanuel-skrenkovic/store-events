using Store.Order.Domain.Payment;
using Xunit;

namespace Store.Order.Domain.Tests
{
    public class PaymentNumberGeneratorTests
    {
        [Fact]
        public void Should_GenerateValidBase64String()
        {
            IPaymentNumberGenerator generator = new Base64PaymentNumberGenerator();

            string paymentNumber = generator.Generate();
            
            Assert.NotNull(paymentNumber);
            Assert.NotEmpty(paymentNumber);
        }
    }
}