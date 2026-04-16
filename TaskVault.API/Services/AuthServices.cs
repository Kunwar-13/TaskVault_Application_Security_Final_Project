using System;
using TaskVault.API.Data;
using Microsoft.Extensions.Configuration;

namespace TaskVault.API.Services;

public interface IAuthService
{

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

}
