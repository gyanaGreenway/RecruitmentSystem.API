using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _context;

    public JobService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<JobDto>> GetAllJobsAsync(int pageNumber, int pageSize)
    {
        var query = _context.Jobs.AsQueryable();
        var totalCount = await query.CountAsync();

        var jobs = await query
            .OrderByDescending(j => j.PostedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JobDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Department = j.Department,
                Location = j.Location,
                Salary = j.Salary,
                PostedDate = j.PostedDate,
                ClosingDate = j.ClosingDate,
                IsActive = j.IsActive
            })
            .ToListAsync();

        return new PagedResultDto<JobDto>
        {
            Items = jobs,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<JobDto?> GetJobByIdAsync(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return null;

        return new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Department = job.Department,
            Location = job.Location,
            Salary = job.Salary,
            PostedDate = job.PostedDate,
            ClosingDate = job.ClosingDate,
            IsActive = job.IsActive
        };
    }

    public async Task<JobDto> CreateJobAsync(CreateJobDto createJobDto)
    {
        var job = new Models.Job
        {
            Title = createJobDto.Title,
            Description = createJobDto.Description,
            Department = createJobDto.Department,
            Location = createJobDto.Location,
            Salary = createJobDto.Salary,
            Requirements = string.Join(",", createJobDto.Requirements ?? new List<string>()),
            PostedDate = DateTime.UtcNow,
            ClosingDate = createJobDto.ClosingDate,
            IsActive = createJobDto.IsActive
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Department = job.Department,
            Location = job.Location,
            Salary = job.Salary,
            PostedDate = job.PostedDate,
            ClosingDate = job.ClosingDate,
            IsActive = job.IsActive
        };
    }

    public async Task<JobDto?> UpdateJobAsync(int id, UpdateJobDto updateJobDto)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return null;

        job.Title = updateJobDto.Title;
        job.Description = updateJobDto.Description;
        job.Department = updateJobDto.Department;
        job.Location = updateJobDto.Location;
        job.Salary = updateJobDto.Salary;
        job.ClosingDate = updateJobDto.ClosingDate;
        job.IsActive = updateJobDto.IsActive;
        job.Requirements = string.Join(",", updateJobDto.Requirements ?? new List<string>());
        job.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Department = job.Department,
            Location = job.Location,
            Salary = job.Salary,
            PostedDate = job.PostedDate,
            ClosingDate = job.ClosingDate,
            IsActive = job.IsActive
        };
    }

    public async Task<bool> DeleteJobAsync(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return false;

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }
}

