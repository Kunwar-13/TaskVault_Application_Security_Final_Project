using System;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TaskVault.API.Data;
using TaskVault.API.DTOs;
using TaskVault.API.Models;

namespace TaskVault.API.Services;

public interface IUploadService
{
 
    Task<(bool Success, string Message)> UploadFileAsync(int userId, FileUploadDto dto);
    Task<IEnumerable<TaskFile>> GetFilesByTaskAsync(int taskId, int userId);

}

public class UploadService : IUploadService
{

    private readonly DbContext _db;
    private readonly string[] _allowedExtensions;
    private readonly long _maxFileSize;
    private readonly string _uploadPath;

    public UploadService(DbContext db, IConfiguration config)
    {
     
        _db = db;

        _allowedExtensions = config.GetSection("FileUploadSettings:AllowedExtensions")
            .Get<string[]>() ?? new[] { ".jpg", ".jpeg", ".png", ".pdf", ".txt", ".csv" };

        _maxFileSize = config.GetValue<long>("FileUploadSettings:MaxFileSizeBytes", 5242880);

        // store uploads outside wwwroot so they are never directly accessible via URL
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        if (!Directory.Exists(_uploadPath){
        
            Directory.CreateDirectory(_uploadPath); 
        
        }
    
    }

    public async Task<(bool Success, string Message)> UploadFileAsync(int userId, FileUploadDto dto)
    {
        
        // verify the task exists and belongs to this user before accepting the file
        using var connection = _db.GetConnection();

        var taskExists = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(1) FROM tasks WHERE id = @TaskId AND user_id = @UserId",
            new { dto.TaskId, UserId = userId }
        );

        if (taskExists == 0){
            
            return (false, "Task not found."); 
        
        }

        var file = dto.File;

        // check file size
        if (file.Length == 0){
            
            return (false, "File is empty."); 
        
        }

        if (file.Length > _maxFileSize){
        
            return (false, $"File exceeds maximum allowed size of {_maxFileSize / 1024 / 1024}MB.");
        
        }
        // check extension against allowlist
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!_allowedExtensions.Contains(extension)){

            return (false, $"File type '{extension}' is not allowed.");

        }

        // verify MIME type matches extension — prevents renaming malicious.exe to safe.jpg
        if (!IsValidMimeType(file.ContentType, extension)){

            return (false, "File content does not match its extension.");

        }

        // generate GUID filename — prevents path traversal and overwrite attacks
        var storedName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_uploadPath, storedName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
           
            await file.CopyToAsync(stream);
        
        }

        // record the file in the database
        await connection.ExecuteAsync(
            @"INSERT INTO task_files (task_id, original_name, stored_name, file_size, mime_type)
              VALUES (@TaskId, @OriginalName, @StoredName, @FileSize, @MimeType)",
            new
            {
                dto.TaskId,
                OriginalName = file.FileName,
                StoredName = storedName,
                FileSize = file.Length,
                MimeType = file.ContentType
            }
        );

        return (true, "File uploaded successfully.");
    
    }

    public async Task<IEnumerable<TaskFile>> GetFilesByTaskAsync(int taskId, int userId)
    {
    
        using var connection = _db.GetConnection();

        // join to tasks to verify ownership before returning file records
        return await connection.QueryAsync<TaskFile>(
            @"SELECT tf.id, tf.task_id, tf.original_name, tf.stored_name,
                     tf.file_size, tf.mime_type, tf.uploaded_at
              FROM task_files tf
              INNER JOIN tasks t ON tf.task_id = t.id
              WHERE tf.task_id = @TaskId AND t.user_id = @UserId",
            new { TaskId = taskId, UserId = userId }
        );
    
    }

    private bool IsValidMimeType(string contentType, string extension)
    {
    
        var allowedMimeMap = new Dictionary<string, string>
        {
            { ".jpg",  "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png",  "image/png"  },
            { ".pdf",  "application/pdf" },
            { ".txt",  "text/plain" },
            { ".csv",  "text/csv"   }
        };

        return allowedMimeMap.TryGetValue(extension, out var expectedMime)
            && contentType.Equals(expectedMime, StringComparison.OrdinalIgnoreCase);
    
    }

}
