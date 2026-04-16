using System;
using System.ComponentModel.DataAnnotations;

namespace TaskVault.API.DTOs;

public class RegisterDto
{
    
    [Required(ErrorMessage = "Username is required")]
    [MinLength (3, ErrorMessage = "Username must be atleast 3 characters")]
    [MaxLength (50, ErrorMessage = "Username cannot exceed 50 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is Required")]
    [EmailAddres(ErrorMessage = "Invalid Email Address Format")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is Required")]
    [MinLength(8, ErrorMessage = "Password must be atleast 8 characters")]
    [MaxLength(100, ErrorMessage = "Password cannot exceed 100 charaters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Password must have uppercase, lowercase, number, and special character")]
    public string Password {  get; set; } = string.Empty;

}
