using System;
using Store.Core.Domain.Functional;

namespace Store.Core.Domain.Result
{
    public class Result<T>
    {
        private readonly Either<Error, T> _either;
        
        public Result(T res)
        {
            _either = Either<Error, T>.FromRight(res);
        }
        
        public Result(Error error)
        {
            _either = Either<Error, T>.FromLeft(error);
        }

        public TResult Match<TResult>(Func<T, TResult> correct, Func<Error, TResult> error) 
            => _either.Match(error, correct);

        public static implicit operator Result<T>(T res) => new(res);
        public static implicit operator Result<T>(Error error) => new(error);
    }
}