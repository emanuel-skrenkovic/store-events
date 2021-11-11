using System;
using Store.Core.Domain.Functional;

namespace Store.Core.Domain.Result
{
    public sealed class Result<T>
    {
        private readonly Either<Error, T> _either;
        
        private Result(T res)
        {
            _either = Either<Error, T>.FromRight(res);
        }
        
        private Result(Error error)
        {
            _either = Either<Error, T>.FromLeft(error);
        }

        public static Result<T> FromValue<T>(T value) => new(value);
        
        public static Result<T> FromError(Error error) => new(error);

        public TResult Match<TResult>(Func<T, TResult> correct, Func<Error, TResult> error) 
            => _either.Match(error, correct);

        public static implicit operator Result<T>(T res) => new(res);
        public static implicit operator Result<T>(Error error) => new(error);
    }
}