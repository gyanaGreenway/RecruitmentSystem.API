using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public interface IJobService
{
    Task<PagedResultDto<JobDto>> GetAllJobsAsync(int pageNumber, int pageSize);
    Task<JobDto?> GetJobByIdAsync(int id);
    Task<JobDto> CreateJobAsync(CreateJobDto createJobDto);
    Task<JobDto?> UpdateJobAsync(int id, UpdateJobDto updateJobDto);
    Task<bool> DeleteJobAsync(int id);
}

