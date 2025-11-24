using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public interface IBackgroundVerificationService
{
    Task<PagedResultDto<BackgroundVerificationDto>> GetAllAsync(int pageNumber = 1, int pageSize = 25, string? status = null);
    Task<BackgroundVerificationDto?> GetByIdAsync(int id);
    Task<BackgroundVerificationDto> CreateAsync(CreateBackgroundVerificationDto dto, int createdById);
    Task<bool> CancelAsync(int id);
    Task<BackgroundVerificationDto?> UpdateStatusAsync(int id, string status, string? notes = null);
}
