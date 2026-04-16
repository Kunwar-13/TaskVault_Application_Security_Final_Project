using System;
using Dapper;
using TaskVault.API.Data;
using TaskVault.API.Models;

namespace TaskVault.API.Services;

public interface ITaskService
{

    Task<IEnumerable<Task>> GetTasksByUserAsync(int userId)

}

public class TaskService : ITaskService
{
    private readonly DbContext _db;

    public TaskService(DbContext db)
    {
     
        _db = db;
    
    }

    public async Task<IEnumerable<Task>> GetTasksByUserAsync(int userId)
    {
     
        using var connection = _db.GetConnection();

        return await connection.QueryAsync<Task>(
            @"SELECT id, user_id, title, description, status, created_at, updated_at
          FROM tasks
          WHERE user_id = @UserId
          ORDER BY created_at DESC",
            new { UserId = userId }
        );
    
    }

}
