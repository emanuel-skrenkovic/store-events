using Store.Core.Domain.Email;
using Store.Core.Domain.Tests.Helpers.Csv;
using Xunit;

namespace Store.Core.Domain.Tests
{
    public class EmailValidatorTests
    {
        [Theory]
        [CsvFileData("./TestCases/Email/ValidEmailAddresses.csv", ",")]
        public void Should_ReturnTrue_On_ValidEmailAddress(string emailAddress)
        {
            IEmailValidator validator = new EmailValidator();
            Assert.True(validator.Validate(emailAddress, out _));
        }

        [Theory]
        [CsvFileData("./TestCases/Email/InvalidEmailAddresses.csv", ",")]
        public void Should_ReturnFalse_On_InvalidEmailAddress(string invalidEmailAddress)
        {
            IEmailValidator validator = new EmailValidator();
            Assert.False(validator.Validate(invalidEmailAddress, out _));
        }
        
        [Theory]
        [CsvFileData("./TestCases/Email/InvalidEmailAddresses.csv", ",")]
        public void Should_OutError_On_InvalidEmailAddress(string invalidEmailAddress)
        {
            IEmailValidator validator = new EmailValidator();
            validator.Validate(invalidEmailAddress, out Error error);
            Assert.NotNull(error);
            Assert.NotEmpty(error.Message);
        }
    }
}