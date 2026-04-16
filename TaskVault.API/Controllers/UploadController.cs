using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskVault.API.DTOs;
using TaskVault.API.Helpers;
using TaskVault.API.Services;

namespace TaskVault.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    
    private readonly IUploadService _uploadService;

    public UploadController(IUploadService uploadService)
    {
    
        _uploadService = uploadService;
    
    }

    [HttpPost]
    [RequestSizeLimit(5242880)] // 5MB hard limit at the HTTP layer before it hits the service
    public async Task<IActionResult> Upload([FromForm] FileUploadDto dto)
    {
    
        var userId = User.GetUserId();
        var (success, message) = await _uploadService.UploadFileAsync(userId, dto);

        if (!success){

            return BadRequest(new { message });
        }
        
        return Ok(new { message });
    
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetFilesForTask(int taskId)
    {
    
        var userId = User.GetUserId();
        var files = await _uploadService.GetFilesByTaskAsync(taskId, userId);

        if (!files.Any()){

            return Ok(new { message = "No files found for this task.", results = Array.Empty<object>() });

        }
        
        return Ok(files);
    
    }

}
