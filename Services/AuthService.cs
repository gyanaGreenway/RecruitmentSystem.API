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

<<<<<<< HEAD
    public async Task<RegisterUserResponseDto?> RegisterAsync(RegisterUserDto dto, UserRole role)
    {
        if (dto == null) return null;

        var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Email = dto.Email.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new RegisterUserResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

=======
>>>>>>> 0dde3112cc163ba687254a43f11c790e5fb680d7
    public async Task<StartPasswordResetResponseDto> StartPasswordResetAsync(StartPasswordResetDto dto)
    {
        var identifier = dto.Identifier.Trim();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == identifier);
        if (user == null)
        {
            // Do not leak whether user exists
<<<<<<< HEAD
            return new StartPasswordResetResponseDto { RequestId = 0, Method = "Unknown" };
=======
            return new StartPasswordResetResponseDto { RequestId =0, Method = "Unknown" };
>>>>>>> 0dde3112cc163ba687254a43f11c790e5fb680d7
        }

        // Choose contact method (email for now). For SMS, you'd look up phone on candidate profile.
        var method = ContactMethod.Email;
        var destination = user.Email;

        // Generate OTP and hash it
        var otp = GenerateNumericOtp(6);
        var otpHash = BCrypt.Net.BCrypt.HashPassword(otp);
        var expires = DateTime.UtcNow.AddMinutes(10);

        var record = new PasswordResetRequest
        {
            UserId = user.Id,
            Method = method,
            Destination = destination,
            OtpHash = otpHash,
            ExpiresAt = expires,
<<<<<<< HEAD
            Attempts = 0,
=======
            Attempts =0,
>>>>>>> 0dde3112cc163ba687254a43f11c790e5fb680d7
            Verified = false,
            ResetToken = Guid.Empty
        };
        _context.PasswordResetRequests.Add(record);
        await _context.SaveChangesAsync();

        // TODO: send OTP via email/SMS provider (out of scope here)
        // e.g., EmailService.Send(destination, $"Your verification code is {otp}")

        return new StartPasswordResetResponseDto
        {
            RequestId = record.Id,
            Method = method.ToString()
        };
    }

    public async Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto dto)
    {
        var rec = await _context.PasswordResetRequests.FirstOrDefaultAsync(r => r.Id == dto.RequestId);
        if (rec == null || rec.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Reset request expired or not found.");
        }
        if (rec.Verified)
        {
            return new VerifyOtpResponseDto { ResetToken = rec.ResetToken, ExpiresAt = rec.ExpiresAt };
        }
        rec.Attempts++;
        if (!BCrypt.Net.BCrypt.Verify(dto.Otp, rec.OtpHash))
        {
            await _context.SaveChangesAsync();
            throw new InvalidOperationException("Invalid verification code.");
        }
        rec.Verified = true;
        rec.ResetToken = Guid.NewGuid();
        rec.ExpiresAt = DateTime.UtcNow.AddMinutes(15); // extend window for reset
        await _context.SaveChangesAsync();

        return new VerifyOtpResponseDto { ResetToken = rec.ResetToken, ExpiresAt = rec.ExpiresAt };
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var rec = await _context.PasswordResetRequests.FirstOrDefaultAsync(r => r.ResetToken == dto.ResetToken && r.Verified);
        if (rec == null || rec.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }
        var user = await _context.Users.FindAsync(rec.UserId);
        if (user == null)
        {
            return false;
        }
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;
        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash)) return false;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AdminChangePasswordAsync(AdminChangePasswordDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null) return false;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    private static string GenerateNumericOtp(int length)
    {
        var rnd = new Random();
        var chars = Enumerable.Range(0, length).Select(_ => rnd.Next(0,10).ToString());
        return string.Concat(chars);
    }

    private string GenerateJwtToken(User user)
    {
        var keyString = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? "RecruitmentSystem";
        var audience = _configuration["Jwt:Audience"] ?? "RecruitmentSystem";

        // Access token lifetime in minutes (default60)
        var accessTokenMinutes =60;
        var configured = _configuration["Jwt:AccessTokenMinutes"];
        if (!string.IsNullOrWhiteSpace(configured) && int.TryParse(configured, out var minutes) && minutes >0)
        {
            accessTokenMinutes = minutes;
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.Name, user.Email) // Include Name claim
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(accessTokenMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

