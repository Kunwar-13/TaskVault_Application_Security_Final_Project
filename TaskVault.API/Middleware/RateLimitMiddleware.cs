using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TaskVault.API.Middleware

public class RateLimitMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _logger;

    // thread-safe dictionary: IP → (request count, window start time)
    private static readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)>
        _requestCounts = new();

    private const int MaxRequests = 5;    // max login attempts
    private const int WindowSeconds = 60;  // per 60 second window
    private const string TargetPath = "/api/auth/login";

    public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
    {
    
        _next = next;
        _logger = logger;
    
    }

    public async Task InvokeAsync(HttpContext context)
    {
    
        // only rate limit the login endpoint
        if (!context.Request.Path.StartsWithSegments(TargetPath) ||
             context.Request.Method != "POST"){
        
            await _next(context);
            return;
        
        }

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var now = DateTime.UtcNow;

        _requestCounts.AddOrUpdate(
            ip,
            // first request from this IP
            _ => (1, now),
            // subsequent requests
            (_, existing) =>
            {
                // reset window if it has expired
                if ((now - existing.WindowStart).TotalSeconds > WindowSeconds){
                    
                    return (1, now);

                }

                return (existing.Count + 1, existing.WindowStart);
            
            }
        );

        var (count, windowStart) = _requestCounts[ip];

        if (count > MaxRequests)
        {
            
            _logger.LogWarning("Rate limit exceeded for IP {IP} on login endpoint", ip);

            context.Response.StatusCode = 429;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Too many login attempts. Please try again later."
            });
            
            return;
        
        }

        await _next(context);
    
    }

}
