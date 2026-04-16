using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskVault.API.Services;
using TaskVault.API.Helpers;

namespace TaskVault.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{

    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
     
        _searchService = searchService;
    
    }

    [HttpGet]
    public async Task<IActionResult> Search(
    [FromQuery] string? q,
    [FromQuery] string? status)
    {
        // reject empty or missing query immediately
        if (string.IsNullOrWhiteSpace(q){
        
            return BadRequest(new { message = "Search query cannot be empty." }); 
        
        }

        var userId = User.GetUserId();
        var results = await _searchService.SearchTasksAsync(userId, q, status);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> Search(
    [FromQuery] string? q,
    [FromQuery] string? status)
    {
        if (string.IsNullOrWhiteSpace(q){
        
            return BadRequest(new { message = "Search query cannot be empty." }); 
        
        }

        // validate status if provided — only accept known values
        var allowedStatuses = new[] { "Pending", "InProgress", "Done" };

        if (!string.IsNullOrWhiteSpace(status) && !allowedStatuses.Contains(status)){
        
            return BadRequest(new { message = "Status must be Pending, InProgress, or Done." }); 

        }

        var userId = User.GetUserId();
        var results = await _searchService.SearchTasksAsync(userId, q, status);

        return Ok(results);
    
    }

}
