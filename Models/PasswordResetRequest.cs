namespace RecruitmentSystem.API.Models;

public enum ContactMethod
{
 Email =1,
 Sms =2
}

public class PasswordResetRequest
{
 public int Id { get; set; }
 public int UserId { get; set; }
 public ContactMethod Method { get; set; }
 public string Destination { get; set; } = string.Empty; // email or phone
 public string OtpHash { get; set; } = string.Empty;
 public DateTime ExpiresAt { get; set; }
 public int Attempts { get; set; }
 public bool Verified { get; set; }
 public Guid ResetToken { get; set; } // issued after OTP is verified
 public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
