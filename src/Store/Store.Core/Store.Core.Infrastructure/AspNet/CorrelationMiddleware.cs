using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Store.Core.Domain;

namespace Store.Core.Infrastructure.AspNet;

public class CorrelationMiddleware
{
    private const string CorrelationIdHeaderName = "correlation-id";
    
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
        => _next = Ensure.NotNull(next);
    
    public Task InvokeAsync(HttpContext httpContext)
    {
        HttpRequest request = httpContext.Request;
        
        string correlationId;
        if (request.Headers.ContainsKey(CorrelationIdHeaderName))
        {
            correlationId = request.Headers[CorrelationIdHeaderName];
        }
        else
        {
            // TODO: Why am I adding request headers on server-side?
            correlationId = Guid.NewGuid().ToString();
            request.Headers.Add(CorrelationIdHeaderName, new[] { correlationId }); 
        }
        
        CorrelationContext.SetCorrelationId(Guid.Parse(correlationId));
        CorrelationContext.SetCausationId(Guid.Parse(correlationId));
        
        return _next(httpContext);
    }
}