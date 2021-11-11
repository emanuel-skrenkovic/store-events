using System;
using System.Threading.Tasks;

namespace Store.Core.Domain.Functional.Extensions
{
    public static class EitherExtensions
    {
        public static Either<L, RR> Bind<L, R, RR>(this Either<L, R> either, Func<R, Either<L, RR>> bind)
        {
            return either.Match(
                left: l => l,
                right: bind); // TODO: does NOT work when L and RR are the same type. FIX IT!
        }

        public static Task<Either<L, RR>> Bind<L, R, RR>(this Either<L, R> either, Func<R, Task<RR>> bind)
        {
            return either.Match<Task<Either<L, RR>>>(
                left: async l => l,
                right: async r => await bind(r));
            
        }  
    }
}