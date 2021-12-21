using System;
using System.Threading.Tasks;
using Store.Core.Domain.ErrorHandling;
using Xunit;

namespace Store.Core.Domain.Tests;

public class ResultTests
{
    private Result<string> ValidResult() => Result.Ok("test_value");

    private Result<string> ErrorResult() => Result.Error<string>(new Error("error_message"));
        
    [Fact]
    private void Result_CreatedSuccessfully_WithValidValue()
    {
        Result<string> result = ValidResult();
        Assert.NotNull(result);
            
        Assert.True(result.IsOk);
        Assert.False(result.IsError);
    }
        
    [Fact]
    private void Result_CreatedSuccessfully_WithNoValue()
    {
        Result result = Result.Ok();
        Assert.NotNull(result);
            
        Assert.True(result.IsOk);
        Assert.False(result.IsError);
    }
        
    [Fact]
    private void Result_CreatedSuccessfully_WithValidError()
    {
        Result<string> stringErrorResult = ErrorResult();
        Assert.NotNull(stringErrorResult);
            
        Assert.True(stringErrorResult.IsError);
        Assert.False(stringErrorResult.IsOk);
    }
        
    [Fact]
    private void Result_CreatedSuccessfully_WithValidError_WithNoValue()
    {
        Result errorResult = Result.Error(new Error("error_message"));
        Assert.NotNull(errorResult);
            
        Assert.True(errorResult.IsError);
        Assert.False(errorResult.IsOk);
    }


    [Fact]
    private void Result_ShouldUnwrapSuccessfully_WithValidValue()
    {
        Result<string> validResult = ValidResult();
        string value = validResult.Unwrap();
        Assert.NotNull(value);
        Assert.NotEmpty(value);
    }

    [Fact]
    private void Result_ShouldThrow_On_Unwrap_WithErrorValue()
    {
        Result<string> errorResult = ErrorResult();
        Assert.Throws<InvalidOperationException>(errorResult.Unwrap);
    }
        
    [Fact]
    private void Result_ShouldThrow_WithProvidedMessage_OnExpect_WithErrorValue()
    {
        Result<string> errorResult = ErrorResult();
        string customErrorMessage = "test_error_message";
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => errorResult.Expect(customErrorMessage));
        Assert.Equal(customErrorMessage, exception.Message);
    }


    [Fact]
    private void Result_ShouldReturnDefault_On_UnwrapOrDefault_WithErrorValue_WithoutProvidedDefaultValue()
    {
        Result<string> errorResult = ErrorResult();
        string value = errorResult.UnwrapOrDefault();
        Assert.Equal(default, value);
    }
        
    [Fact]
    private void Result_ShouldReturnProvidedValue_On_UnwrapOrDefault_WithErrorValue_WithProvidedDefaultValue()
    {
        Result<string> errorResult = ErrorResult();
        string defaultValue = "test";
        string value = errorResult.UnwrapOrDefault(defaultValue);
        Assert.Equal(defaultValue, value);
    }
        
    [Fact]
    private void Result_ShouldOnlyRunOkAction_On_Match_WithValidResult_WithoutValue()
    {
        Result validResult = Result.Ok();

        bool validRan = false;
        validResult.Match(() => validRan = true, _ => throw new Exception());
        Assert.True(validRan);
    }

    [Fact]
    private async Task Result_ShouldOnlyRunOkAsyncAction_on_Match_WithValidResult_WithoutValue()
    {
        Result validResult = Result.Ok();

        bool validRan = false;
        await validResult.Match(
            async () =>
            {
                await Task.Delay(1);
                validRan = true;
            }, 
            _ => throw new Exception());
        Assert.True(validRan);
    }
        
    [Fact]
    private void Result_ShouldOnlyRunErrorAction_On_Match_WithErrorValue()
    {
        Result errorResult = Result.Error(new Error("error_message"));

        bool errorRan = false;
        errorResult.Match(() => throw new Exception(), _ => errorRan = true);
        Assert.True(errorRan);
    }

    [Fact]
    private async Task Result_ShouldOnlyRunErrorAsyncAction_On_Match_WithErrorValue()
    {
        Result errorResult = Result.Error(new Error("error_message"));

        bool errorRan = false;
        await errorResult.Match(
            () => throw new Exception(), 
            async _ =>
            {
                await Task.Delay(1);
                errorRan = true;
            });
        Assert.True(errorRan); 
    }

    [Fact]
    private void ResultT_ShouldOnlyRunOkAction_On_Match_WithValidValue()
    {
        Result<string> validResult = ValidResult();

        bool validRan = false;
        validResult.Match(_ => validRan = true, _ => throw new Exception());
        Assert.True(validRan);
    }

    [Fact]
    private async Task ResultT_ShouldOnlyRunOkAsyncAction_On_Match_WithValidValue()
    {
        Result<string> validResult = ValidResult();

        bool validRan = false;
        await validResult.Match(
            async _ =>
            {
                await Task.Delay(1);
                validRan = true;
            }, _ => throw new Exception());
        Assert.True(validRan);
    }
        
    [Fact]
    private void ResultT_ShouldOnlyRunErrorAction_On_Match_WithErrorValue()
    {
        Result<string> errorResult = ErrorResult();

        bool errorRan = false;
        errorResult.Match(_ => throw new Exception(), _ => errorRan = true);
        Assert.True(errorRan);
    }
        
    [Fact]
    private async Task ResultT_ShouldOnlyRunErrorAsyncAction_On_Match_WithErrorValue()
    {
        Result<string> errorResult = ErrorResult();

        bool errorRan = false;
        await errorResult.Match(
            _ => throw new Exception(), 
            async _ =>
            {
                await Task.Delay(1);
                errorRan = true;
            });
        Assert.True(errorRan); 
    }

    [Fact]
    private void Result_ShouldReturnNewValidResult_OnThen_WithValidValue()
    {
        Result<string> validResult = ValidResult();

        int newValue = 2;
        Result<int> newValidResult = validResult.Then(_ => newValue);
        Assert.True(newValidResult.IsOk);
        Assert.Equal(newValue, newValidResult.Unwrap());
    }
        
    [Fact]
    private void Result_ShouldReturnNewErrorResult_OnThen_WithErrorValue()
    {
        Result<string> validResult = ErrorResult();

        int newValue = 2;
        Result<int> newValidResult = validResult.Then(_ => newValue);
        Assert.True(newValidResult.IsError);
    }
}