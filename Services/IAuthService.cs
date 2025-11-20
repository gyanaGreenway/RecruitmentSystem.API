using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
<<<<<<< HEAD
    Task<RegisterUserResponseDto?> RegisterAsync(RegisterUserDto dto, UserRole role);
=======
>>>>>>> 0dde3112cc163ba687254a43f11c790e5fb680d7
    Task<StartPasswordResetResponseDto> StartPasswordResetAsync(StartPasswordResetDto dto);
    Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto dto);
    Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<bool> AdminChangePasswordAsync(AdminChangePasswordDto dto);
}

