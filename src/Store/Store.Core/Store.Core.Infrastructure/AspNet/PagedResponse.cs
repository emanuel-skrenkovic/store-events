using System.Collections.Generic;
using System.Linq;
using Store.Core.Domain;

namespace Store.Core.Infrastructure.AspNet;

public class PagedResponse<T>
{
    public T[] Data { get; }
    
    public string Cursor { get; }

    public PagedResponse(
        ICollection<T> data, 
        string cursor)
    {
        Ensure.NotNull(data);
        
        Data     = data.ToArray();
        Cursor   = cursor;
    }
}