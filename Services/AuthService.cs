using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecruitmentSystem.API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        if (loginDto == null) return null;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return null;
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
        if (!passwordValid)
        {
            return null;
        }

        var token = GenerateJwtToken(user);

        return new LoginResponseDto
        {
            Token = token,
            Role = user.Role.ToString(),
            UserId = user.Id,
            Email = user.Email
        };
    }

    private string GenerateJwtToken(User user)
    {
        var keyString = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? "RecruitmentSystem";
        var audience = _configuration["Jwt:Audience"] ?? "RecruitmentSystem";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

