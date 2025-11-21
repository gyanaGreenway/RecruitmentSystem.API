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
                PublicId = j.PublicId,
                Title = j.Title,
                Description = j.Description,
                Department = j.Department,
                Location = j.Location,
                Salary = j.Salary,
                PostedDate = j.PostedDate,
                ClosingDate = j.ClosingDate,
                IsActive = j.IsActive,
                Requirements = (j.Requirements ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList()
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
            PublicId = job.PublicId,
            Title = job.Title,
            Description = job.Description,
            Department = job.Department,
            Location = job.Location,
            Salary = job.Salary,
            PostedDate = job.PostedDate,
            ClosingDate = job.ClosingDate,
            IsActive = job.IsActive,
            Requirements = (job.Requirements ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList()
        };
    }

    public async Task<JobDto> CreateJobAsync(CreateJobDto createJobDto)
    {
        // Prevent duplicate titles (case-insensitive)
        var duplicateTitle = await _context.Jobs
            .AnyAsync(j => j.Title.ToLower() == createJobDto.Title.ToLower());
        if (duplicateTitle)
        {
            throw new InvalidOperationException("A job with the same title already exists.");
        }

        var job = new Models.Job
        {
            PublicId = Guid.NewGuid(),
            Title = createJobDto.Title,
            Description = createJobDto.Description,
            Department = createJobDto.Department,
            Location = createJobDto.Location,
            Salary = createJobDto.Salary,
            Requirements = string.Join(',', createJobDto.Requirements ?? new List<string>()),
            PostedDate = DateTime.UtcNow,
            ClosingDate = createJobDto.ClosingDate,
            IsActive = createJobDto.IsActive
        };

        _context.Jobs.Add(job);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Enforced by DB unique index
            throw new InvalidOperationException("A job with the same title already exists.");
        }

        return new JobDto
        {
            Id = job.Id,
            PublicId = job.PublicId,
            Title = job.Title,
            Description = job.Description,
            Department = job.Department,
            Location = job.Location,
            Salary = job.Salary,
            PostedDate = job.PostedDate,
            ClosingDate = job.ClosingDate,
            IsActive = job.IsActive,
            Requirements = (job.Requirements ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList()
        };
    }

    public async Task<JobDto?> UpdateJobAsync(int id, UpdateJobDto updateJobDto)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return null;

        // Prevent updating to a duplicate title (case-insensitive) for other rows
        var duplicateTitle = await _context.Jobs
            .AnyAsync(j => j.Id != id && j.Title.ToLower() == updateJobDto.Title.ToLower());
        if (duplicateTitle)
        {
            throw new InvalidOperationException("A job with the same title already exists.");
        }

        job.Title = updateJobDto.Title;
        job.Description = updateJobDto.Description;
        job.Department = updateJobDto.Department;
        job.Location = updateJobDto.Location;
        job.Salary = updateJobDto.Salary;
        job.ClosingDate = updateJobDto.ClosingDate;
        job.IsActive = updateJobDto.IsActive;
        job.Requirements = string.Join(',', updateJobDto.Requirements ?? new List<string>());
        job.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Enforced by DB unique index
            throw new InvalidOperationException("A job with the same title already exists.");
        }

        return new JobDto
        {
            Id = job.Id,
            PublicId = job.PublicId,
            Title = job.Title,
            Description = job.Description,
            Department = job.Department,
            Location = job.Location,
            Salary = job.Salary,
            PostedDate = job.PostedDate,
            ClosingDate = job.ClosingDate,
            IsActive = job.IsActive,
            Requirements = (job.Requirements ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList()
        };
    }

    public async Task<bool> DeleteJobAsync(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return false;

        var relatedApplications = await _context.Applications.Where(a => a.JobId == id).ToListAsync();
        if (relatedApplications.Count > 0 && job.IsActive)
        {
            // Active job: protect from accidental destructive delete
            throw new InvalidOperationException("Cannot delete active job while applications exist. Set IsActive to false first.");
        }

        if (relatedApplications.Count > 0)
        {
            // Job already inactive => remove related applications then delete job
            _context.Applications.RemoveRange(relatedApplications);
        }

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }
}

// public async Task<bool> DeleteJobAsync(int id)
//    {
//        var job = await _context.Jobs.FindAsync(id);
//        if (job == null) return false;

//        // Prevent deletion if applications reference this job
//        var hasApplications = await _context.Applications.AnyAsync(a => a.JobId == id);
//        if (hasApplications)
//        {
//            throw new InvalidOperationException("Cannot delete job because applications exist. Set IsActive to false instead or remove related applications first.");
//        }

//        _context.Jobs.Remove(job);
//        await _context.SaveChangesAsync();
//        return true;
//    }
//}