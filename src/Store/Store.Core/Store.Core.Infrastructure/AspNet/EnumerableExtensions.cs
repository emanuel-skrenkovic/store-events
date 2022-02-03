using System;
using System.Collections.Generic;
using System.Linq;

namespace Store.Core.Infrastructure.AspNet;

public static class EnumerableExtensions
{
    public static TKey NextCursor<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> selector, int limit)
        where TKey : class
    {
        var arr = enumerable.ToArray();
        if (arr.Length <= limit) return null;

        return selector(arr.Last());
    }
    
    /// <summary>
    /// Used for convenience of getting the next cursor from a cursor
    /// paged enumerable. This variant is used for specifically working with
    /// nullable struct types.
    /// </summary>
    /// <param name="enumerable"></param>
    /// <param name="selector"></param>
    /// <param name="limit"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static TKey? NextValueCursor<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> selector, int limit)
        where TKey : struct
    {
        var arr = enumerable.ToArray();
        if (arr.Length <= limit) return null;

        return selector(arr.Last());
    }
    
    public static PagedResponse<T> ToPagedResponse<T>(this IEnumerable<T> input, PagingParams pagingParams)
    {
        (string previousPageQuery, string nextPageQuery) = pagingParams.ToQueryStrings();

        return new PagedResponse<T>(
            input.Take(pagingParams.Limit).ToArray(),
            nextPageQuery,
            previousPageQuery);
    }
}