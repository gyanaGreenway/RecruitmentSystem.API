using System.ComponentModel.DataAnnotations;

namespace RecruitmentSystem.API.DTOs;

public class PublicApplicationDto
{
    [Required]
    public int JobId { get; set; }

    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(20)]
    [RegularExpression(@"^[+]?\d{7,20}$", ErrorMessage = "Phone must be digits (optionally leading +) length 7-20.")]
    public string Phone { get; set; } = string.Empty;

    //[Required, StringLength(500)]
    public string? ResumeUrl { get; set; }

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? KeySkills { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
