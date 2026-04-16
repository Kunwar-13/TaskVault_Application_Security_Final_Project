using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TaskVault.API.DTOs;

public class FileUploadDto
{
    [Required(ErrorMessage = "File is required")]
    public IFormFile File { get; set; } = null!;

    [Required(ErrorMessage = "TaskId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "TaskId must be a positive number")]
    public int TaskId { get; set; }

}
