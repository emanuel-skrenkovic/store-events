using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Store.Core.Domain;

namespace Store.Core.Infrastructure.AspNet;

public class CorrelationMiddleware
{
    private const string MessageIdHeaderName = "message-id";
    private const string CorrelationIdHeaderName = "correlation-id";
    
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
        => _next = Ensure.NotNull(next);
    
    public Task InvokeAsync(HttpContext httpContext)
    {
        HttpRequest request = httpContext.Request;

        MapHeader(request, CorrelationIdHeaderName, out string correlationId);
        CorrelationContext.SetCorrelationId(Guid.Parse(correlationId));
        CorrelationContext.SetCausationId(Guid.Parse(correlationId));
        
        MapHeader(request, MessageIdHeaderName, out string messageId);
        CorrelationContext.SetMessageId(Guid.Parse(messageId));
        
        return _next(httpContext);
    }

    private void MapHeader(HttpRequest request, string headerName, out string value)
    {
        // TODO: is this case sensitive?
        if (request.Headers.ContainsKey(headerName))
        {
            value = request.Headers[headerName];
        }
        else
        {
            // TODO: Why am I adding request headers on server-side?
            value = Guid.NewGuid().ToString();
            request.Headers.Add(headerName, new[] { value }); 
        } 
    }
}