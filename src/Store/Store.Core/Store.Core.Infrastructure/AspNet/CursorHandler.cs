using System;
using System.Text;
using Store.Core.Domain;

namespace Store.Core.Infrastructure.AspNet;

public class CursorHandler
{
    private readonly ISerializer _serializer;

    public CursorHandler(ISerializer serializer)
        => _serializer = Ensure.NotNull(serializer);
    
    public T Parse<T>(string composedCursor)
    {
        Ensure.NotNullOrEmpty(composedCursor);

        string decoded = Encoding.UTF8.GetString(
            Convert.FromBase64String(composedCursor));
        return _serializer.Deserialize<T>(decoded);
    }

    public string Compose<T>(T data)
    {
        if (data == null) return null;
        
        return Convert.ToBase64String(
            _serializer.SerializeToBytes(data));
    }
}