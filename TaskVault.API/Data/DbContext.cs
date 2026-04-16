using System;
using Microsoft.Extensions.Configuration;

namespace TaskVault.API.Data;

public class DbContext
{

    private readonly string _connectionString;

    public DbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

}
