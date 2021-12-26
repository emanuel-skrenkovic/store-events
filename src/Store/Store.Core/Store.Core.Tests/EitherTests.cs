using System;
using Store.Core.Domain.Functional;
using Store.Core.Domain.Functional.Extensions;
using Xunit;

namespace Store.Core.Tests;

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

    [Fact]
    public void Should_BindResult_When_IsRight()
    {
        bool rightExecuted = false;

        int value = 2;
        Either<Unit, int> testEither = Either<Unit, int>.FromRight(value);
            
        Either<Unit, string> bindResult = testEither.Bind<Unit, int, string>(i => i.ToString());
            
        bindResult.Match(l => throw new Exception(), r =>
        {
            rightExecuted = true;
            Assert.Equal(value.ToString(), r);
            return Unit.Value;
        });
            
        Assert.True(rightExecuted);
    }
        
    [Fact]
    public void Should_BindError_When_IsLeft()
    {
        bool leftExecuted = false;

        Either<Unit, int> testEither = Either<Unit, int>.FromLeft(Unit.Value);
           
        Either<Unit, string> bindResult = testEither.Bind<Unit, int, string>(i => i.ToString());

        bindResult.Match(l =>
        {
            leftExecuted = true;
            return Unit.Value;
        }, r => throw new Exception());

        Assert.True(leftExecuted);
    }
}