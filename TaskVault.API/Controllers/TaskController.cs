using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskVault.API.Services;
using TaskVault.API.Helpers;
using TaskVault.API.DTOs;

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

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskDto dto)
    {
     
        var userId = User.GetUserId();
        var newId = await _taskService.CreateTaskAsync(userId, dto);

        return CreatedAtAction(
            nameof(GetTask),
            new { id = newId },
            new { message = "Task created.", taskId = newId }
        );
    
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto dto)
    {
        
        var userId = User.GetUserId();
        var success = await _taskService.UpdateTaskAsync(id, userId, dto);

        if (!success)
            return NotFound(new { message = "Task not found." });

        return Ok(new { message = "Task updated." });
    
    }

}
