using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskVault.API.Services;

namespace TaskVault.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
    
        _adminService = adminService;
    
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
    
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    
    }

    [HttpPatch("users/{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
    
        var success = await _adminService.DeactivateUserAsync(id);

        if (!success){

            return BadRequest(new { message = "User not found or cannot deactivate an admin account." });

        }
        
        return Ok(new { message = "User deactivated." });
    
    }

    [HttpPatch("users/{id}/activate")]
    public async Task<IActionResult> ActivateUser(int id)
    {

        var success = await _adminService.ActivateUserAsync(id);

        if (!success){ 
        
            return NotFound(new { message = "User not found." }); 
        
        }

        return Ok(new { message = "User activated." });
    
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) {
            
            page = 1; 
        
        }
        
        if (pageSize < 1) { 
        
            pageSize = 20; 
        
        }
        
        if (pageSize > 100) { 
        
            pageSize = 100; 
        
        } // cap to prevent huge responses

        var logs = await _adminService.GetAuditLogsAsync(page, pageSize);
        return Ok(logs);
    
    }

    [HttpGet("logs/user/{userId}")]
    public async Task<IActionResult> GetLogsByUser(int userId)
    {
    
        var logs = await _adminService.GetAuditLogsByUserAsync(userId);

        if (!logs.Any()){
         
            return Ok(new { message = "No logs found for this user.", results = Array.Empty<object>() });
        
        }

        return Ok(logs);
    
    }

}
