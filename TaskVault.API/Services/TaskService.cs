using System;
using Dapper;
using TaskVault.API.Data;
using TaskVault.API.Models;
using TaskVault.API.DTOs;

namespace TaskVault.API.Services;

public interface ITaskService
{

    Task<IEnumerable<Task>> GetTasksByUserAsync(int userId);
    Task<Task?> GetTaskByIdAsync(int taskId, int userId);
    Task<int> CreateTaskAsync(int userId, TaskDto dto);
    Task<bool> UpdateTaskAsync(int taskId, int userId, TaskDto dto);
    Task<bool> DeleteTaskAsync(int taskId, int userId);

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

    public async Task<Task?> GetTaskByIdAsync(int taskId, int userId)
    {
    
        using var connection = _db.GetConnection();

        return await connection.QueryFirstOrDefaultAsync<Task>(
            @"SELECT id, user_id, title, description, status, created_at, updated_at
          FROM tasks
          WHERE id = @TaskId AND user_id = @UserId",
            new { TaskId = taskId, UserId = userId }
        );
    
    }

    public async Task<int> CreateTaskAsync(int userId, TaskDto dto)
    {

        using var connection = _db.GetConnection();

        var newId = await connection.QuerySingleAsync<int>(
            @"INSERT INTO tasks (user_id, title, description, status)
          VALUES (@UserId, @Title, @Description, @Status);
          SELECT SCOPE_IDENTITY();",
            new
            {
                UserId = userId,
                dto.Title,
                dto.Description,
                dto.Status
            }
        );

        return newId;

    }

    public async Task<bool> UpdateTaskAsync(int taskId, int userId, TaskDto dto)
    {
       
        using var connection = _db.GetConnection();

        var rows = await connection.ExecuteAsync(
            @"UPDATE tasks
          SET title       = @Title,
              description = @Description,
              status      = @Status,
              updated_at  = @UpdatedAt
          WHERE id = @TaskId AND user_id = @UserId",
            new
            {
                dto.Title,
                dto.Description,
                dto.Status,
                UpdatedAt = DateTime.UtcNow,
                TaskId = taskId,
                UserId = userId
            }
        );

        // rows affected = 0 means task not found or not owned by this user
        return rows > 0;
    
    }

    public async Task<bool> DeleteTaskAsync(int taskId, int userId)
    {
       
        using var connection = _db.GetConnection();

        var rows = await connection.ExecuteAsync(
            @"DELETE FROM tasks
          WHERE id = @TaskId AND user_id = @UserId",
            new { TaskId = taskId, UserId = userId }
        );

        return rows > 0;
    
    }

}
