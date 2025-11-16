using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public interface IApplicationService
{
    Task<PagedResultDto<ApplicationDto>> GetApplicationsAsync(ApplicationFilterDto filter);
    Task<ApplicationDto?> GetApplicationByIdAsync(int id);
    Task<ApplicationDto> CreateApplicationAsync(CreateApplicationDto createApplicationDto);
    Task<ApplicationDto?> UpdateApplicationStatusAsync(int id, UpdateApplicationStatusDto updateDto, string? changedBy = null);
}

