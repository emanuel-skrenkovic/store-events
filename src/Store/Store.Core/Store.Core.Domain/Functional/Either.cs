using System;

namespace Store.Core.Domain.Functional
{
    public class Either<TL, TR>
    {
        private readonly TL _left;
        private readonly TR _right;
        
        public bool IsLeft { get; }
        public bool IsRight => !IsLeft;

        private Either(TL left)
        {
            IsLeft = true;
            _left = left;
            _right = default;
        }
        
        private Either(TR right)
        {
            IsLeft = false;
            _right = right;
            _left = default;
        }

        public static Either<TL, TR> FromLeft(TL left) => new(left);
        public static Either<TL, TR> FromRight(TR right) => new(right);

        public T Match<T>(Func<TL, T> left, Func<TR, T> right)
            => IsLeft ? left(_left) : right(_right);

        public static implicit operator Either<TL, TR>(TL left) => FromLeft(left);
        public static implicit operator Either<TL, TR>(TR right) => FromRight(right);
    }
}