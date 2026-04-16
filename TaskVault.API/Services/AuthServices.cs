using System;
using TaskVault.API.Data;
using Microsoft.Extensions.Configuration;

namespace TaskVault.API.Services;

public interface IAuthService
{

    string HashPassword(string plainPassword);

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

}
