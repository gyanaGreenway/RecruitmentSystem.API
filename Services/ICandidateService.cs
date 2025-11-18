using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public interface ICandidateService
{
    Task<PagedResultDto<CandidateDto>> GetAllCandidatesAsync(int pageNumber, int pageSize);
    Task<CandidateDto?> GetCandidateByIdAsync(int id);
    Task<CandidateDto> CreateCandidateAsync(CreateCandidateDto createCandidateDto);
    Task<CandidateDto?> UpdateCandidateAsync(int id, UpdateCandidateDto updateCandidateDto);
    Task<bool> DeleteCandidateAsync(int id);
    Task<List<CandidateDto>> SearchCandidatesAsync(string query);
    Task<int> StartCandidateRegistrationAsync(StartCandidateRegistrationDto dto);
    Task<CandidateDto> ConfirmCandidateRegistrationAsync(ConfirmCandidateRegistrationDto dto);
}

