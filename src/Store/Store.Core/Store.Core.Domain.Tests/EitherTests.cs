using System;
using Store.Core.Domain.Functional;
using Xunit;

namespace Store.Core.Domain.Tests
{
    public class EitherTests
    {
        [Fact]
        public void Should_BeLeft_When_CreatedFromLeft()
        {
            Either<int, string> testEither = Either<int, string>.FromLeft(2);
            Assert.True(testEither.IsLeft);
        }
        
        [Fact]
        public void Should_BeRight_When_CreatedFromRight()
        {
            Either<int, string> testEither = Either<int, string>.FromRight("test");
            Assert.True(testEither.IsRight);
        }
        
        [Fact]
        public void Should_MatchLeft_When_IsLeft()
        {
            bool leftExecuted = false;

            Either<int, string> testEither = Either<int, string>.FromLeft(2);
            testEither.Match(l => leftExecuted = true, r => throw new Exception());
            
            Assert.True(leftExecuted);
        }
        
        [Fact]
        public void Should_MatchRight_When_IsRight()
        {
            bool rightExecuted = false;

            Either<int, string> testEither = Either<int, string>.FromRight("test");
            testEither.Match(l => throw new Exception(), r => rightExecuted = true);
            
            Assert.True(rightExecuted);
        }
    }
}