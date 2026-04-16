using System;
using System.ComponentModel.DataAnnotations;

namespace TaskVault.API.DTOs;

public class TaskDto
{
    
    [Required(ErrorMessage = "Title is required")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;

}
