using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<RegisterUserResponseDto?> RegisterAsync(RegisterUserDto dto, UserRole role);
    Task<StartPasswordResetResponseDto> StartPasswordResetAsync(StartPasswordResetDto dto);
    Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto dto);
    Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<bool> AdminChangePasswordAsync(AdminChangePasswordDto dto);
}

