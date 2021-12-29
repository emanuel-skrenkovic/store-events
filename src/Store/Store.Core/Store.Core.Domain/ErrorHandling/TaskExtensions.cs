using System;
using System.Threading.Tasks;

namespace Store.Core.Domain.ErrorHandling;

public static class TaskExtensions
{
    public static async Task<Result> Then(this Task<Result> task, Func<Task<Result>> ok)
    {
        Result result = await task;
        if (result.IsError) return result;
        
        return await ok();
    }
    
    public static async Task<Result<TResult>> Then<TResult>(
        this Task<Result> task, 
        Func<Task<Result<TResult>>> ok)
    {
        Result result = await task;
        return await result.Then(ok);
    }
    
    public static async Task<Result<TResult>> Then<TResult>(
        this Task<Result> task, 
        Func<Result<TResult>> ok)
    {
        Result result = await task;
        return result.Then(ok);
    }

    public static async Task<Result<TR>> Then<T, TR>(this Task<Result<T>> task, Func<T, Task<Result<TR>>> ok)
    {
        Result<T> result = await task;
        return await result.Then(ok);
    }

    public static async Task<Result<TR>> Then<T, TR>(this Task<Result<T>> task, Func<T, Result<TR>> ok)
    {
        Result<T> result = await task;
        return result.Then(ok); 
    }
    
    public static async Task<Result> Then<T>(
        this Task<Result<T>> task, 
        Func<T, Task<Result>> ok)
    {
        Result<T> result = await task;
        return await result.Then(ok);
    }
}