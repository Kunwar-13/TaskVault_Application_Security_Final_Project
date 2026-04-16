using System;
using TaskVault.API.Services;
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

}
