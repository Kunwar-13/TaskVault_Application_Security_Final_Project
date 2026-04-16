using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskVault.API.Services;

namespace TaskVault.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{

    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
     
        _taskService = taskService;
    
    }

}
