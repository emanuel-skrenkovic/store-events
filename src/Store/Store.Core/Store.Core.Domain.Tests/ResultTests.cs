using System;
using Store.Core.Domain.Result;
using Xunit;

namespace Store.Core.Domain.Tests
{
    public class ResultTests
    {
        [Fact]
        public void Should_MatchValue_When_NoError()
        {
            bool correctExecuted = false;

            int value = 2;

            Result<int> testResult = new(value);
            testResult.Match(val =>
            {
                Assert.Equal(value, val);
                return correctExecuted = true;
            }, _ => throw new Exception());
            
            Assert.True(correctExecuted);
        }
        
        [Fact]
        public void Should_MatchRight_When_IsRight()
        {
            bool errorExecuted = false;

            string errorMessage = "test_message";
            Error error = new Error(errorMessage);

            Result<int> testResult = new(error);
            testResult.Match(_ => throw new Exception(), err =>
            {
                Assert.Equal(errorMessage, err.Message);
                return errorExecuted = true;
            });
            
            Assert.True(errorExecuted);
        }
    }
}