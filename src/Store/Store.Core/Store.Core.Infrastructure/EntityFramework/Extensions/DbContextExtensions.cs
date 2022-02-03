using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Core.Infrastructure.EntityFramework.Entity;

namespace Store.Core.Infrastructure.EntityFramework.Extensions;

public enum CursorDirection
{
    Descending,
    Ascending
}

public static class DbContextExtensions
{
    public static IQueryable<TEntity> After<TEntity, TKey>
    (
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, TKey>> keySelectorExpression,
        object cursor
    )
        where TEntity : class
        where TKey : IComparable
    {
        if (cursor != null)
        {
            var parameterExpr = Expression.Parameter(typeof(TEntity), "e");
            var cursorExpr = Expression.Constant(cursor);

            BinaryExpression comparisonExpr = Expression.LessThanOrEqual(
                Expression.Invoke(keySelectorExpression, parameterExpr), 
                cursorExpr); 
            
            var filterExpr = Expression.Lambda<Func<TEntity, bool>>(comparisonExpr, parameterExpr);

            query = query.Where(filterExpr);
        }

        return query.OrderByDescending(keySelectorExpression);
    }

    public static async Task<TKey> PreviousCursorOrDefaultAsync<TEntity, TKey>
    (
        this IQueryable<TEntity> query, 
        Expression<Func<TEntity, TKey>> keySelectorExpression,
        object cursor, 
        int limit
    )
        where TEntity : class
        where TKey : IComparable
    {
        if (cursor is null) return default;
        
        var parameterExpr = Expression.Parameter(typeof(TEntity), "e");

        var cursorExpr = Expression.Constant((TKey)cursor);
        BinaryExpression comparisonExpr = Expression.GreaterThan(
            Expression.Invoke(keySelectorExpression, parameterExpr), 
            cursorExpr);  
            
        var filterExpr = Expression.Lambda<Func<TEntity, bool>>(comparisonExpr, parameterExpr);

        // TODO: currently don't know how to do it without pulling
        // in the entire previous page.
        TEntity last =  await query
            .Where(filterExpr)
            .OrderBy(keySelectorExpression)
            .Take(limit)
            .LastOrDefaultAsync();

        return last is null ? default : keySelectorExpression.Compile()(last);
    }
    
    public static async Task<TKey?> PreviousValueCursorOrDefaultAsync<TEntity, TKey>
    (
        this IQueryable<TEntity> query, 
        Expression<Func<TEntity, TKey>> keySelectorExpression,
        object cursor, 
        int limit
    )
        where TEntity : class
        where TKey : struct
    {
        if (cursor is null) return default;
        
        var parameterExpr = Expression.Parameter(typeof(TEntity));

        var cursorExpr = Expression.Constant((TKey)cursor);
        BinaryExpression comparisonExpr = Expression.GreaterThan(
            Expression.Invoke(keySelectorExpression, parameterExpr), 
            cursorExpr);  
            
        var filterExpr = Expression.Lambda<Func<TEntity, bool>>(comparisonExpr, parameterExpr);

        // TODO: currently don't know how to do it without pulling
        // in the entire previous page.
        TEntity last =  await query
            .Where(filterExpr)
            .OrderBy(keySelectorExpression)
            .Take(limit)
            .LastOrDefaultAsync();

        return last is null ? null : keySelectorExpression.Compile()(last);
    }
    
    public static IQueryable<TEntity> Before<TEntity, TKey>
    (
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, TKey>> keySelectorExpression,
        object cursor
    )
        where TEntity : class
        where TKey : IComparable
    {
        if (cursor != null)
        {
            var parameterExpr = Expression.Parameter(typeof(TEntity), "e");

            var cursorExpr = Expression.Constant(cursor);
            BinaryExpression comparisonExpr = Expression.GreaterThan(
                    Expression.Invoke(keySelectorExpression, parameterExpr), 
                    cursorExpr);  
            
            var filterExpr = Expression.Lambda<Func<TEntity, bool>>(comparisonExpr, parameterExpr);

            query = query.Where(filterExpr);
        }

        return query.OrderBy(keySelectorExpression);
    }

    public static IQueryable<TEntity> From<TEntity, TKey>
    (
        this IQueryable<TEntity> query, 
        Expression<Func<TEntity, TKey>> keySelectorExpression,
        object cursor,
        CursorDirection direction = CursorDirection.Descending
    )
        where TEntity : class
        where TKey : IComparable
    {
        if (cursor != null)
        {
            var parameterExpr = Expression.Parameter(typeof(TEntity), "e");

            var cursorExpr = Expression.Constant(cursor);
            BinaryExpression comparisonExpr;

            if (CursorDirection.Descending == direction)
            {
                comparisonExpr = Expression.LessThanOrEqual(
                    Expression.Invoke(keySelectorExpression, parameterExpr), 
                    cursorExpr); 
            }
            else
            {
                comparisonExpr = Expression.GreaterThan(
                    Expression.Invoke(keySelectorExpression, parameterExpr), 
                    cursorExpr);  
            }
            
            var filterExpr = Expression.Lambda<Func<TEntity, bool>>(comparisonExpr, parameterExpr);

            query = query.Where(filterExpr);
        }

        return CursorDirection.Descending == direction 
            ? query.OrderByDescending(keySelectorExpression)
            : query.OrderBy(keySelectorExpression);
    }
    
    public static async Task<ulong> GetSubscriptionCheckpoint(this DbContext context, string subscriptionId)
    {
        SubscriptionCheckpointEntity checkpoint = await context.Set<SubscriptionCheckpointEntity>()
            .FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionId);

        return checkpoint?.Position ?? 0;
    }

    /// <summary>
    /// Calls context.Add or context.Update for the checkpoint.
    /// Does NOT save the changes.
    /// </summary>
    public static async Task AddOrUpdateSubscriptionCheckpoint(this DbContext context, string subscriptionId, ulong newPosition)
    {
        SubscriptionCheckpointEntity checkpoint = await context.Set<SubscriptionCheckpointEntity>()
            .FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionId);

        if (checkpoint == null)
        {
            checkpoint = new() { Id = Guid.NewGuid(), SubscriptionId = subscriptionId, Position = newPosition };
            context.Add(checkpoint);
        }
        else
        {
            checkpoint.Position = newPosition;
            context.Update(checkpoint);
        }
    }
}