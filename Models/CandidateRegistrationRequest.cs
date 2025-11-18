namespace RecruitmentSystem.API.Models;

public class CandidateRegistrationRequest
{
 public int Id { get; set; }
 public string FirstName { get; set; } = string.Empty;
 public string LastName { get; set; } = string.Empty;
 public string Email { get; set; } = string.Empty;
 public string Phone { get; set; } = string.Empty;
 public string? ResumeUrl { get; set; }
 public string KeySkills { get; set; } = string.Empty;
 public string PasswordHash { get; set; } = string.Empty;

 public string OtpHash { get; set; } = string.Empty;
 public DateTime ExpiresAt { get; set; }
 public bool Verified { get; set; }
 public int Attempts { get; set; }
 public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
