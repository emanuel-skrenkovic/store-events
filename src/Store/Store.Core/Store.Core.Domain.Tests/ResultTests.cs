using System;
using Store.Core.Domain.Functional;
using Store.Core.Domain.Result;
using Store.Core.Domain.Result.Extensions;
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

            Result<int> testResult = Result<int>.FromValue(value);
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

            Result<int> testResult = Result<int>.FromError(error);
            testResult.Match(_ => throw new Exception(), err =>
            {
                Assert.Equal(errorMessage, err.Message);
                return errorExecuted = true;
            });
            
            Assert.True(errorExecuted);
        }

        [Fact]
        public void Should_BindValue_When_IsSuccess()
        {
            int value = 2;

            Result<int> testResult = Result<int>.FromValue(value);
            Result<string> bindResult = testResult.Bind<int, string>(i => i.ToString());
            
            bindResult.Match(val =>
            {
                Assert.Equal(value.ToString(), val);
                return Unit.Value;
            }, _ => throw new Exception());
        }
        
        [Fact]
        public void Should_BindError_When_IsError()
        {
            bool errorPropagated = false;
            
            Result<int> testResult = Result<int>.FromError(new Error("error_message"));
            Result<string> bindResult = testResult.Bind<int, string>(i => i.ToString());
            
            bindResult.Match(val => throw new Exception(),
                err => 
            {
                Assert.NotNull(err);
                errorPropagated = true;
                return Unit.Value;
            });
            
            Assert.True(errorPropagated);
        }
    }
}