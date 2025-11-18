using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.Services;

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;

    public ApplicationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<ApplicationDto>> GetApplicationsAsync(ApplicationFilterDto filter)
    {
        var query = _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Candidate)
            .AsQueryable();

        // Apply filters
        if (filter.Status.HasValue)
        {
            query = query.Where(a => a.Status == filter.Status.Value);
        }

        if (filter.JobId.HasValue)
        {
            query = query.Where(a => a.JobId == filter.JobId.Value);
        }

        if (filter.CandidateId.HasValue)
        {
            query = query.Where(a => a.CandidateId == filter.CandidateId.Value);
        }

        if (filter.StartDate.HasValue)
        {
            query = query.Where(a => a.AppliedDate >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(a => a.AppliedDate <= filter.EndDate.Value);
        }

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "applieddate" => filter.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(a => a.AppliedDate)
                : query.OrderByDescending(a => a.AppliedDate),
            "status" => filter.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(a => a.Status)
                : query.OrderByDescending(a => a.Status),
            "job" => filter.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(a => a.Job.Title)
                : query.OrderByDescending(a => a.Job.Title),
            _ => query.OrderByDescending(a => a.AppliedDate)
        };

        var totalCount = await query.CountAsync();

        var applications = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(a => new ApplicationDto
            {
                Id = a.Id,
                JobId = a.JobId,
                CandidateId = a.CandidateId,
                Status = a.Status,
                AppliedDate = a.AppliedDate,
                Notes = a.Notes,
                Job = new JobDto
                {
                    Id = a.Job.Id,
                    Title = a.Job.Title,
                    Description = a.Job.Description,
                    Department = a.Job.Department,
                    Location = a.Job.Location,
                    Salary = a.Job.Salary,
                    PostedDate = a.Job.PostedDate,
                    ClosingDate = a.Job.ClosingDate,
                    IsActive = a.Job.IsActive
                },
                Candidate = new CandidateDto
                {
                    Id = a.Candidate.Id,
                    FirstName = a.Candidate.FirstName,
                    LastName = a.Candidate.LastName,
                    Email = a.Candidate.Email,
                    Phone = a.Candidate.Phone,
                    ResumeUrl = a.Candidate.ResumeUrl
                }
            })
            .ToListAsync();

        return new PagedResultDto<ApplicationDto>
        {
            Items = applications,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<ApplicationDto?> GetApplicationByIdAsync(int id)
    {
        var application = await _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Candidate)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null) return null;

        return new ApplicationDto
        {
            Id = application.Id,
            JobId = application.JobId,
            CandidateId = application.CandidateId,
            Status = application.Status,
            AppliedDate = application.AppliedDate,
            Notes = application.Notes,
            Job = new JobDto
            {
                Id = application.Job.Id,
                Title = application.Job.Title,
                Description = application.Job.Description,
                Department = application.Job.Department,
                Location = application.Job.Location,
                Salary = application.Job.Salary,
                PostedDate = application.Job.PostedDate,
                ClosingDate = application.Job.ClosingDate,
                IsActive = application.Job.IsActive
            },
            Candidate = new CandidateDto
            {
                Id = application.Candidate.Id,
                FirstName = application.Candidate.FirstName,
                LastName = application.Candidate.LastName,
                Email = application.Candidate.Email,
                Phone = application.Candidate.Phone,
                ResumeUrl = application.Candidate.ResumeUrl
            }
        };
    }

    public async Task<ApplicationDto> CreateApplicationAsync(CreateApplicationDto createApplicationDto)
    {
        // Check for existing application
        var existingApplication = await _context.Applications
            .FirstOrDefaultAsync(a => a.JobId == createApplicationDto.JobId && 
                                      a.CandidateId == createApplicationDto.CandidateId);

        if (existingApplication != null)
        {
            if (existingApplication.Status == ApplicationStatus.Rejected)
            {
                using var reopenTx = await _context.Database.BeginTransactionAsync();
                try
                {
                    var previousStatus = existingApplication.Status;
                    existingApplication.Status = ApplicationStatus.New;
                    existingApplication.AppliedDate = DateTime.UtcNow;
                    if (!string.IsNullOrWhiteSpace(createApplicationDto.Notes))
                    {
                        existingApplication.Notes = createApplicationDto.Notes;
                    }
                    await _context.SaveChangesAsync();
                    _context.ApplicationStatusHistories.Add(new ApplicationStatusHistory
                    {
                        ApplicationId = existingApplication.Id,
                        PreviousStatus = previousStatus,
                        NewStatus = ApplicationStatus.New,
                        ChangedAt = DateTime.UtcNow,
                        Notes = "Candidate reapplied"
                    });
                    await _context.SaveChangesAsync();
                    await reopenTx.CommitAsync();
                    return await GetApplicationByIdAsync(existingApplication.Id) ?? throw new Exception("Failed to reload application");
                }
                catch
                {
                    await reopenTx.RollbackAsync();
                    throw;
                }
            }

            // Any non-rejected existing application blocks a new one
            throw new InvalidOperationException("Application already exists for this job and candidate.");
        }

        // Load job to capture current cycle
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == createApplicationDto.JobId);
        if (job == null) throw new InvalidOperationException("Job not found.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var application = new Application
            {
                JobId = createApplicationDto.JobId,
                CandidateId = createApplicationDto.CandidateId,
                Cycle = job.Cycle,
                Status = ApplicationStatus.New,
                AppliedDate = DateTime.UtcNow,
                Notes = createApplicationDto.Notes
            };

            _context.Applications.Add(application);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Possible race condition: unique constraint at DB level
                var dup = await _context.Applications
                    .AnyAsync(a => a.JobId == createApplicationDto.JobId && a.CandidateId == createApplicationDto.CandidateId && a.Cycle == job.Cycle);
                if (dup)
                {
                    throw new InvalidOperationException("Application already exists for this job and candidate.");
                }

                throw;
            }

            // Create initial status history
            var statusHistory = new ApplicationStatusHistory
            {
                ApplicationId = application.Id,
                PreviousStatus = ApplicationStatus.New,
                NewStatus = ApplicationStatus.New,
                ChangedAt = DateTime.UtcNow,
                Notes = "Application created"
            };

            _context.ApplicationStatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return await GetApplicationByIdAsync(application.Id) ?? throw new Exception("Failed to retrieve created application");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ApplicationDto?> UpdateApplicationStatusAsync(int id, UpdateApplicationStatusDto updateDto, string? changedBy)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null) return null;

        // Validate status transition
        var validTransitions = new Dictionary<ApplicationStatus, List<ApplicationStatus>>
        {
            { ApplicationStatus.New, new List<ApplicationStatus> { ApplicationStatus.Shortlisted, ApplicationStatus.Rejected } },
            { ApplicationStatus.Shortlisted, new List<ApplicationStatus> { ApplicationStatus.Hired, ApplicationStatus.Rejected } },
            { ApplicationStatus.Rejected, new List<ApplicationStatus> { ApplicationStatus.New } } // allow reopen
        };

        if (validTransitions.ContainsKey(application.Status) &&
            !validTransitions[application.Status].Contains(updateDto.Status))
        {
            throw new InvalidOperationException($"Invalid status transition from {application.Status} to {updateDto.Status}");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var previousStatus = application.Status;
            application.Status = updateDto.Status;
            if (!string.IsNullOrEmpty(updateDto.Notes))
            {
                application.Notes = updateDto.Notes;
            }

            await _context.SaveChangesAsync();

            // Save status change history
            var statusHistory = new ApplicationStatusHistory
            {
                ApplicationId = application.Id,
                PreviousStatus = previousStatus,
                NewStatus = updateDto.Status,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = changedBy,
                Notes = updateDto.Notes
            };

            _context.ApplicationStatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return await GetApplicationByIdAsync(application.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ApplicationStatusHistoryDto>> GetStatusHistoryAsync(int applicationId)
    {
        return await _context.ApplicationStatusHistories
            .Where(h => h.ApplicationId == applicationId)
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new ApplicationStatusHistoryDto
            {
                Id = h.Id,
                ApplicationId = h.ApplicationId,
                PreviousStatus = h.PreviousStatus,
                NewStatus = h.NewStatus,
                ChangedAt = h.ChangedAt,
                ChangedBy = h.ChangedBy,
                Notes = h.Notes
            })
            .ToListAsync();
    }
}

