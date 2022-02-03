using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Store.Core.Domain;

namespace Store.Core.Infrastructure.AspNet;

public class PagingParams
{
    private const string CursorQueryParameterName = "cursor";
    
    private readonly IEnumerable<KeyValuePair<string, StringValues>> _otherQueryParameters;
    
    public string Next { get; }
    public string Previous { get; }

    public int Limit { get; }
    
    public PagingParams
    (
        object next, 
        object previous, 
        int limit,
        IEnumerable<KeyValuePair<string, StringValues>> otherQueryParameters = null
    )
    {
        Ensure.Positive(limit);
        
        Next = Encode(next);
        Previous = Encode(previous);
        Limit = limit;
        _otherQueryParameters = otherQueryParameters;
    }

    public (string, string) ToQueryStrings(string cursorQueryParameterName = CursorQueryParameterName)
    {
        List<KeyValuePair<string, StringValues>> queryParameters = _otherQueryParameters
            .Where(kv => kv.Key != cursorQueryParameterName)
            .ToList(); 
        
        QueryBuilder nextQueryBuilder = null;
        if (Next != null)
        {
            nextQueryBuilder = new();

            foreach (var (key, value) in queryParameters)
            {
                if (value.Any())
                {
                    nextQueryBuilder.Add(key, value.AsEnumerable());
                }
            }
            
            nextQueryBuilder.Add(cursorQueryParameterName, Next);
        }
        
        QueryBuilder previousQueryBuilder = null;
        if (Previous != null)
        {
            previousQueryBuilder = new();
            
            foreach (var (key, value) in queryParameters)
            {
                if (value.Any())
                {
                    previousQueryBuilder.Add(key, value.AsEnumerable());
                }
            }
            previousQueryBuilder.Add(cursorQueryParameterName, Previous);
        }
        
        return (
            nextQueryBuilder?.ToString(), 
            previousQueryBuilder?.ToString()
        );
    }

    private string Encode(object cursor)
    {
        if (cursor is null) return null;
        
        return Convert.ToBase64String(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(cursor));
    }
}