using System;
using System.Data;
using Microsoft.Data.SqlClient;
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

    public IDbConnection GetConnection()
    {
     
        return new SqlConnection(_connectionString);
    
    }

}
