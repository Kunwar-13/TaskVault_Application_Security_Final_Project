using System;
using System.ComponentModel.DataAnnotations;

namespace TaskVault.API.DTOs;

public class LoginDto
{
    
    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

}
