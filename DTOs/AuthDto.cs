using System.ComponentModel.DataAnnotations;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // string name
    public int RoleCode { get; set; } // numeric enum value
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int? CandidateId { get; set; } // nullable if HR
}

public class RegisterUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterUserResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

