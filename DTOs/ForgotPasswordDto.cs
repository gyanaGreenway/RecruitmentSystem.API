using System.ComponentModel.DataAnnotations;

namespace RecruitmentSystem.API.DTOs;

public class StartPasswordResetDto
{
 [Required]
 public string Identifier { get; set; } = string.Empty; // email or phone
}

public class StartPasswordResetResponseDto
{
 public int RequestId { get; set; }
 public string Method { get; set; } = string.Empty; // Email or Sms
}

public class VerifyOtpDto
{
 [Required]
 public int RequestId { get; set; }
 [Required]
 [StringLength(8, MinimumLength =4)]
 public string Otp { get; set; } = string.Empty;
}

public class VerifyOtpResponseDto
{
 public Guid ResetToken { get; set; }
 public DateTime ExpiresAt { get; set; }
}

public class ResetPasswordDto
{
 [Required]
 public Guid ResetToken { get; set; }
 [Required]
 [StringLength(100, MinimumLength =8)]
 public string NewPassword { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
 [Required]
 [StringLength(100, MinimumLength =8)]
 public string CurrentPassword { get; set; } = string.Empty;

 [Required]
 [StringLength(100, MinimumLength =8)]
 public string NewPassword { get; set; } = string.Empty;
}

public class AdminChangePasswordDto
{
 [Required]
 public int UserId { get; set; }

 [Required]
 [StringLength(100, MinimumLength =8)]
 public string NewPassword { get; set; } = string.Empty;
}
