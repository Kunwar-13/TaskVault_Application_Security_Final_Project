using System;
using TaskVault.API.Services;
using TaskVault.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace TaskVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
     
        _authService = authService;
    
    }

    [HttpPost("register")]
    
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
    
        var success = await _authService.RegisterAsync(dto);

        if (!success){
        
            return Conflict(new { message = "Username or email already exists." }); 
        
        }

        return Ok(new { message = "Registration successful." });
    
    }

}
