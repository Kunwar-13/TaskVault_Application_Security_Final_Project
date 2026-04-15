using System;

namespace TaskVault.API.Models;

public class User
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

}