using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskVault.API.Services;
using TaskVault.API.Helpers;

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

    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
     
        var userId = User.GetUserId();
        var tasks = await _taskService.GetTasksByUserAsync(userId);
        return Ok(tasks);
    
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
     
        var userId = User.GetUserId();
        var task = await _taskService.GetTaskByIdAsync(id, userId);

        if (task == null)
            return NotFound(new { message = "Task not found." });

        return Ok(task);
    
    }

}
