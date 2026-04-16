using System;
using TaskVault.API.Data;
using Microsoft.Extensions.Configuration;

namespace TaskVault.API.Services;

public interface IAuthService
{

    string HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, string hash);

}

public class AuthService : IAuthService
{
   
    private readonly DbContext _db;
    private readonly IConfiguration _config;

    public AuthService(DbContext db, IConfiguration config)
    {
    
        _db = db;
        _config = config;
    
    }

    public string HashPassword(string plainPassword)
    {
     
        return BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12);
    
    }

    public bool VerifyPassword(string plainPassword, string hash)
    {
     
        return BCrypt.Net.BCrypt.Verify(plainPassword, hash);
    
    }

}
