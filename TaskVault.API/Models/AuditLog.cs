using System;

namespace TaskVault.API.Models;

public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }

    public string IpAddress { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Path {  get; set; } = string.Empty;
    public string StatusCode { get; set; }

    public string? EventNote { get; set; }
    public DateTime CreatedAt { get; set; }

}
