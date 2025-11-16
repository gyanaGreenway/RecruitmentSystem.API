using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
}

