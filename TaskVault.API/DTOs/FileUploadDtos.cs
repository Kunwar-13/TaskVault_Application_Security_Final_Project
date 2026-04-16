using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TaskVault.API.DTOs;

public class FileUploadDto
{
    [Required(ErrorMessage = "File is required")]
    public IFormFile File { get; set; } = null!;


}
