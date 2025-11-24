using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public interface IOnboardingService
{
    Task<PagedOnboardingResult> GetAllAsync(OnboardingFilterDto filter);
    Task<OnboardingDto?> GetByIdAsync(int id);
    Task<OnboardingDto> CreateAsync(CreateOnboardingDto dto);
    Task<OnboardingDto?> UpdateAsync(int id, UpdateOnboardingDto dto);
    Task<bool> DeleteAsync(int id);
    Task<OnboardingTaskDto> AddTaskAsync(int onboardingId, CreateOnboardingTaskDto dto);
    Task<OnboardingTaskDto?> UpdateTaskAsync(int taskId, UpdateOnboardingTaskDto dto);
    Task<bool> DeleteTaskAsync(int taskId);
    Task<OnboardingDocumentDto> UploadDocumentAsync(int onboardingId, UploadOnboardingDocumentDto dto, int uploadedById);
    Task<bool> DeleteDocumentAsync(int documentId);
}
