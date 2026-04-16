using System;
using Dapper;
namespace TaskVault.API.Services;
using TaskVault.API.Data;
using TaskVault.API.Models;

public interface ISearchService
{

    Task<IEnumerable<Task>> SearchTasksAsync(int userId, string query, string? status);

}

public class SearchService : ISearchService
{

    private readonly DbContext _db;

    public SearchService(DbContext db)
    {
     
        _db = db;
    
    }

    public async Task<IEnumerable<Task>> SearchTasksAsync(int userId, string query, string? status)
    {
       
        using var connection = _db.GetConnection();

        // build sql conditionally but always parameterized
        // never concatenate user input into the query string directly
        var sql = @"SELECT id, user_id, title, description, status, created_at, updated_at
                FROM tasks
                WHERE user_id = @UserId
                AND (title LIKE @Query OR description LIKE @Query)";

        // optionally filter by status if provided
        if (!string.IsNullOrWhiteSpace(status))
            sql += " AND status = @Status";

        sql += " ORDER BY created_at DESC";

        return await connection.QueryAsync<Task>(
            sql,
            new
            {
                UserId = userId,
                Query = $"%{query}%",  // wrapped in % for LIKE matching
                Status = status
            }
        );
    
    }

    private string SanitizeQuery(string query)
    {
        
        if (string.IsNullOrWhiteSpace(query){
        
            return string.Empty; 
        
        }

        // trim whitespace
        query = query.Trim();

        // cap length to prevent excessively long queries hitting the DB
        if (query.Length > 100)
            query = query[..100];

        // remove characters that have no place in a search term
        var invalidChars = new[] { ';', '\'', '"', '\\', '\0' };
        foreach (var c in invalidChars)
            query = query.Replace(c.ToString(), string.Empty);

        return query;
    }

}
