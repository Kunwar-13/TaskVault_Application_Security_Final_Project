using System;
using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Http;
using TaskVault.API.Data;

namespace TaskVault.API.Middleware;

public class AuditLoggingMiddleware
{
   
    private readonly RequestDelegate _next;

    public AuditLoggingMiddleware(RequestDelegate next)
    {
    
        _next = next;
    
    }

    public async Task InvokeAsync(HttpContext context, DbContext db)
    {
    
        // let the request complete first so we can log the response status code
        await _next(context);

        try
        {
        
            var userId = GetUserId(context);
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();
            var status = context.Response.StatusCode;
            var note = GetEventNote(path, method, status);

            using var connection = db.GetConnection();

            await connection.ExecuteAsync(
                @"INSERT INTO audit_logs (user_id, ip_address, method, path, status_code, event_note)
                  VALUES (@UserId, @IpAddress, @Method, @Path, @StatusCode, @EventNote)",
                new
                {
                    UserId = userId,
                    IpAddress = ip,
                    Method = method,
                    Path = path,
                    StatusCode = status,
                    EventNote = note
                }
            );
        
        }
        catch
        {
        
            // never let logging failure break the application
            // silently swallow logging errors
        
        }
    
    }

    private int? GetUserId(HttpContext context)
    {
    
        var claim = context.User?.FindFirst(ClaimTypes.NameIdentifier)
                 ?? context.User?.FindFirst("sub");

        if (claim == null) {
        
            return null; 
        
        }

        return int.TryParse(claim.Value, out var id) ? id : null;
    
    }

    private string? GetEventNote(string path, string method, int status)
    {
        // tag security-relevant events with a readable note
        if (path.Contains("/auth/login") && status == 401) { return "Failed login attempt"; }

        if (path.Contains("/auth/login") && status == 200) { return "Successful login"; }

        if (path.Contains("/auth/register") && status == 409) { return "Duplicate registration attempt"; }

        if (path.Contains("/upload") && status == 400) { return "File upload rejected"; }

        if (path.Contains("/admin") && status == 403) { return "Unauthorized admin access attempt"; }

        if (status == 429) { return "Rate limit exceeded"; }

        return null;
    
    }

}
