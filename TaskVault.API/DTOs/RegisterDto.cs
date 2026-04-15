using System;
using System.ComponentModel.DataAnnotations;

namespace TaskVault.API.DTOs;

public class RegisterDto
{
    
    [Required(ErrorMessage = "Username is required")]
    [MinLength (3, ErrorMessage = "Username must be atleast 3 characters")]
    [MaxLength (50, ErrorMessage = "Username cannot exceed 50 characters")]

    public string Username { get; set; } = string.Empty;

}
