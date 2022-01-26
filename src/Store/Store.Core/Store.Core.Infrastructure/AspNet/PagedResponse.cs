using System.Collections.Generic;
using System.Linq;
using Store.Core.Domain;

namespace Store.Core.Infrastructure.AspNet;

public class PagedResponse<T>
{
    public T[] Data { get; }
    
    public int PageSize { get; }
    
    public string Cursor { get; }

    public PagedResponse(
        ICollection<T> data, 
        int pageSize, 
        string cursor)
    {
        Ensure.NotNull(data);
        Ensure.Positive(pageSize);
        Ensure.NotNullOrEmpty(cursor);
        
        Data     = data.ToArray();
        PageSize = pageSize;
        Cursor   = cursor;
    }
}