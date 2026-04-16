using System;
using Dapper;
using TaskVault.API.Data;
using TaskVault.API.Models;

namespace TaskVault.API.Services;

public interface IAdminService
{
    
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> DeactivateUserAsync(int userId);
    Task<bool> ActivateUserAsync(int userId);
    Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int page, int pageSize);
    Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(int userId);

}

public class AdminService : IAdminService
{

    private readonly DbContext _db;

    public AdminService(DbContext db)
    {

        _db = db;

    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {

        using var connection = _db.GetConnection();

        // never return password_hash to the admin panel
        return await connection.QueryAsync<User>(
            @"SELECT id, username, email, role, is_active, created_at
              FROM users
              ORDER BY created_at DESC"
        );

    }

    public async Task<bool> DeactivateUserAsync(int userId)
    {

        using var connection = _db.GetConnection();

        var rows = await connection.ExecuteAsync(
            @"UPDATE users
              SET is_active = 0
              WHERE id = @UserId AND role != 'Admin'",
            new { UserId = userId }
        );

        // role != Admin guard means admin accounts can never be deactivated
        // through this endpoint even if someone finds a way to call it
        return rows > 0;

    }

    public async Task<bool> ActivateUserAsync(int userId)
    {

        using var connection = _db.GetConnection();

        var rows = await connection.ExecuteAsync(
            @"UPDATE users
              SET is_active = 1
              WHERE id = @UserId",
            new { UserId = userId }
        );

        return rows > 0;

    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int page, int pageSize)
    {

        using var connection = _db.GetConnection();

        // paginated so the admin panel does not load thousands of rows at once
        var offset = (page - 1) * pageSize;

        return await connection.QueryAsync<AuditLog>(
            @"SELECT id, user_id, ip_address, method, path, status_code, event_note, created_at
              FROM audit_logs
              ORDER BY created_at DESC
              OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
            new { Offset = offset, PageSize = pageSize }
        );

    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(int userId)
    {

        using var connection = _db.GetConnection();

        return await connection.QueryAsync<AuditLog>(
            @"SELECT id, user_id, ip_address, method, path, status_code, event_note, created_at
              FROM audit_logs
              WHERE user_id = @UserId
              ORDER BY created_at DESC",
            new { UserId = userId }
        );

    }

}
