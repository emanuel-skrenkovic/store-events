using System;
using System.Threading.Tasks;

namespace Store.Core.Domain.Result.Extensions
{
    public static class ResultExtensions
    {
        public static Result<TT> Bind<T, TT>(this Result<T> result, Func<T, Result<TT>> bind)
        {
            return result.Match(
                correct: bind,
                error: err => err);
        }

        public static Task<Result<TT>> Bind<T, TT>(this Result<T> result, Func<T, Task<Result<TT>>> bind)
        {
            return result.Match<Task<Result<TT>>>(
                correct: async r => await bind(r),
                error: async l => l);

        }  
    }
}