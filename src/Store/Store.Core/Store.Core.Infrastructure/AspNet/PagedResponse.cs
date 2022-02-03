using System.Collections.Generic;
using System.Linq;
using Store.Core.Domain;

namespace Store.Core.Infrastructure.AspNet;

public class PagedResponse<T>
{
    public T[] Data { get; internal set; }
    
    public string Previous { get; internal set; }
    
    public string Next { get; internal set; }

    public PagedResponse(
        IEnumerable<T> data, 
        string previous,
        string next)
    {
        T[] dataArray = data?.ToArray();
        Ensure.NotNull(dataArray);

        Data = dataArray;
        Previous = previous;
        Next = next;
    }
}

public static class PagedResponseExtensions
{
    private static CursorHandler CursorHandler = new(new JsonSerializer());
    
    public static PagedResponse<T> WithData<T>(this PagedResponse<T> pagedResponse, IEnumerable<T> data)
    {
        pagedResponse.Data = data.ToArray();
        return pagedResponse;
    }
    
    public static PagedResponse<T> WithNext<T>(this PagedResponse<T> pagedResponse, string cursor)
    {
        return pagedResponse;
    }
    
    public static PagedResponse<T> Previous<T>(this PagedResponse<T> pagedResponse, string cursor)
    {
        return pagedResponse;
    }
}