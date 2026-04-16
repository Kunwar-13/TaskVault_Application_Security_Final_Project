using System;
using TaskVault.API.Data;
using TaskVault.API.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskVault.API.Models;
using TaskVault.API.DTOs;
using Dapper;
using TaskVault.API.DTOs;


namespace TaskVault.API.Services;

public interface IAuthService
{

    string GenerateJwt(User user);
    string HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, string hash);
    Task<bool> RegisterAsync(RegisterDto dto);

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

    public string GenerateJwt(User user)
    {
       
        var secret = _config["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT secret not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiry = int.Parse(_config["JwtSettings:ExpiryMinutes"] ?? "60");

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiry),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
      
        using var connection = _db.GetConnection();

        // check if email or username already exists
        var existing = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(1) FROM users WHERE email = @Email OR username = @Username",
            new { dto.Email, dto.Username }
        );

        if (existing > 0{
        
            return false; 
        
        }

        var hash = HashPassword(dto.Password);

        await connection.ExecuteAsync(
            @"INSERT INTO users (username, email, password_hash, role)
          VALUES (@Username, @Email, @PasswordHash, 'User')",
            new { dto.Username, dto.Email, PasswordHash = hash }
        );

        return true;
    
    }

}
