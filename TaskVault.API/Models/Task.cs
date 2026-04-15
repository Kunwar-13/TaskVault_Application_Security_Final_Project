using System;

namespace TaskVault.API.Models;

public class Task
{

    public int Id { get; set; }
    public int UserId { get; set; }

    public string Ttile { get; set; } = string.Empty;
    public string? Description { get; set; }

}
