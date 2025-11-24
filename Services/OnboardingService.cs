using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.Services;

public class OnboardingService : IOnboardingService
{
    private readonly ApplicationDbContext _context;
    private readonly IEncryptionService _encryption;

    public OnboardingService(ApplicationDbContext context, IEncryptionService encryption)
    {
        _context = context;
        _encryption = encryption;
    }

    private OnboardingDto Map(Onboarding o)
    {
        return new OnboardingDto
        {
            Id = o.Id,
            PublicId = o.PublicId,
            CandidateId = o.CandidateId,
            CandidateName = string.Empty,
            JobId = o.JobId,
            JobTitle = string.Empty,
            RecruiterId = o.RecruiterId,
            RecruiterName = string.Empty,
            StartDate = o.StartDate,
            DueDate = o.DueDate,
            Status = o.Status.ToString(),
            Notes = o.Notes,
            TasksCount = o.TasksCount,
            CompletedTasksCount = o.Tasks.Count(t => t.Completed),
            DocumentsCount = o.DocumentsCount,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            Tasks = o.Tasks.Select(t => new OnboardingTaskDto
            {
                Id = t.Id,
                PublicId = t.PublicId,
                Title = t.Title,
                Description = t.Description,
                AssignedToId = t.AssignedToId,
                AssignedToName = string.Empty,
                DueDate = t.DueDate,
                Completed = t.Completed,
                CompletedAt = t.CompletedAt,
                Notes = t.Notes,
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList() ?? new List<OnboardingTaskDto>(),
            Documents = o.Documents.Select(d => new OnboardingDocumentDto
            {
                Id = d.Id,
                PublicId = d.PublicId,
                FileName = d.FileName,
                Url = string.IsNullOrEmpty(d.Url) ? string.Empty : _encryption.Unprotect(d.Url),
                FileType = d.FileType,
                FileSize = d.FileSize,
                Status = d.Status.ToString(),
                Comments = d.Comments,
                UploadedById = d.UploadedById,
                UploadedByName = string.Empty,
                UploadedAt = d.UploadedAt
            }).ToList() ?? new List<OnboardingDocumentDto>()
        };
    }

    public async Task<PagedOnboardingResult> GetAllAsync(OnboardingFilterDto filter)
    {
        var query = _context.Onboardings
            .Include(o => o.Tasks)
            .Include(o => o.Documents)
            .AsQueryable();

        if (filter.CandidateId.HasValue) query = query.Where(o => o.CandidateId == filter.CandidateId.Value);
        if (filter.JobId.HasValue) query = query.Where(o => o.JobId == filter.JobId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<OnboardingStatus>(filter.Status, true, out var status))
            query = query.Where(o => o.Status == status);
        if (filter.RecruiterId.HasValue) query = query.Where(o => o.RecruiterId == filter.RecruiterId.Value);
        if (filter.StartDate.HasValue) query = query.Where(o => o.StartDate >= filter.StartDate.Value);
        if (filter.EndDate.HasValue) query = query.Where(o => o.StartDate <= filter.EndDate.Value);

        var totalCount = await query.CountAsync();
        var entities = await query
            .OrderByDescending(o => o.StartDate)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var items = entities.Select(o => Map(o)).ToList();

        return new PagedOnboardingResult { Items = items, TotalCount = totalCount, PageNumber = filter.PageNumber, PageSize = filter.PageSize };
    }

    public async Task<OnboardingDto?> GetByIdAsync(int id)
    {
        var onboarding = await _context.Onboardings
            .Include(o => o.Tasks)
            .Include(o => o.Documents)
            .FirstOrDefaultAsync(o => o.Id == id);
        return onboarding == null ? null : Map(onboarding);
    }

    public async Task<OnboardingDto> CreateAsync(CreateOnboardingDto dto)
    {
        var onboarding = new Onboarding
        {
            CandidateId = dto.CandidateId,
            JobId = dto.JobId,
            RecruiterId = dto.RecruiterId,
            StartDate = DateTime.UtcNow,
            DueDate = dto.DueDate,
            Status = OnboardingStatus.Pending,
            Notes = dto.Notes,
            TasksCount = dto.Tasks.Count
        };
        _context.Onboardings.Add(onboarding);
        await _context.SaveChangesAsync();

        foreach (var taskDto in dto.Tasks)
        {
            _context.OnboardingTasks.Add(new OnboardingTask
            {
                OnboardingId = onboarding.Id,
                Title = taskDto.Title,
                Description = taskDto.Description,
                AssignedToId = taskDto.AssignedToId,
                DueDate = taskDto.DueDate,
                Completed = false
            });
        }
        await _context.SaveChangesAsync();

        return await GetByIdAsync(onboarding.Id) ?? Map(onboarding);
    }

    public async Task<OnboardingDto?> UpdateAsync(int id, UpdateOnboardingDto dto)
    {
        var onboarding = await _context.Onboardings.FindAsync(id);
        if (onboarding == null) return null;

        if (dto.DueDate.HasValue) onboarding.DueDate = dto.DueDate;
        if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<OnboardingStatus>(dto.Status, true, out var status))
            onboarding.Status = status;
        if (!string.IsNullOrWhiteSpace(dto.Notes)) onboarding.Notes = dto.Notes;
        onboarding.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var onboarding = await _context.Onboardings.FindAsync(id);
        if (onboarding == null) return false;
        _context.Onboardings.Remove(onboarding);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<OnboardingTaskDto> AddTaskAsync(int onboardingId, CreateOnboardingTaskDto dto)
    {
        var task = new OnboardingTask
        {
            OnboardingId = onboardingId,
            Title = dto.Title,
            Description = dto.Description,
            AssignedToId = dto.AssignedToId,
            DueDate = dto.DueDate,
            Completed = false
        };
        _context.OnboardingTasks.Add(task);
        var onboarding = await _context.Onboardings.FindAsync(onboardingId);
        if (onboarding != null) onboarding.TasksCount++;
        await _context.SaveChangesAsync();

        return new OnboardingTaskDto
        {
            Id = task.Id,
            PublicId = task.PublicId,
            Title = task.Title,
            Description = task.Description,
            AssignedToId = task.AssignedToId,
            AssignedToName = string.Empty,
            DueDate = task.DueDate,
            Completed = task.Completed,
            CompletedAt = task.CompletedAt,
            Notes = task.Notes
        };
    }

    public async Task<OnboardingTaskDto?> UpdateTaskAsync(int taskId, UpdateOnboardingTaskDto dto)
    {
        var task = await _context.OnboardingTasks.FindAsync(taskId);
        if (task == null) return null;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.AssignedToId = dto.AssignedToId;
        task.DueDate = dto.DueDate;
        var wasCompleted = task.Completed;
        task.Completed = dto.Completed;
        if (dto.Completed && !wasCompleted) task.CompletedAt = DateTime.UtcNow;
        task.Notes = dto.Notes;

        await _context.SaveChangesAsync();
        return new OnboardingTaskDto
        {
            Id = task.Id,
            PublicId = task.PublicId,
            Title = task.Title,
            Description = task.Description,
            AssignedToId = task.AssignedToId,
            AssignedToName = string.Empty,
            DueDate = task.DueDate,
            Completed = task.Completed,
            CompletedAt = task.CompletedAt,
            Notes = task.Notes
        };
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var task = await _context.OnboardingTasks.FindAsync(taskId);
        if (task == null) return false;
        var onboarding = await _context.Onboardings.FindAsync(task.OnboardingId);
        if (onboarding != null && onboarding.TasksCount > 0) onboarding.TasksCount--;
        _context.OnboardingTasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<OnboardingDocumentDto> UploadDocumentAsync(int onboardingId, UploadOnboardingDocumentDto dto, int uploadedById)
    {
        var doc = new OnboardingDocument
        {
            OnboardingId = onboardingId,
            FileName = dto.FileName,
            Url = string.IsNullOrEmpty(dto.Url) ? string.Empty : _encryption.Protect(dto.Url), // Protect URL before saving
            FileType = dto.FileType,
            FileSize = dto.FileSize,
            Status = OnboardingDocumentStatus.Pending,
            Comments = dto.Comments ?? string.Empty,
            UploadedById = uploadedById,
            UploadedAt = DateTime.UtcNow
        };
        _context.OnboardingDocuments.Add(doc);
        var onboarding = await _context.Onboardings.FindAsync(onboardingId);
        if (onboarding != null) onboarding.DocumentsCount++;
        await _context.SaveChangesAsync();

        return new OnboardingDocumentDto
        {
            Id = doc.Id,
            PublicId = doc.PublicId,
            FileName = doc.FileName,
            Url = string.IsNullOrEmpty(doc.Url) ? string.Empty : _encryption.Unprotect(doc.Url),
            FileType = doc.FileType,
            FileSize = doc.FileSize,
            Status = doc.Status.ToString(),
            Comments = doc.Comments,
            UploadedById = doc.UploadedById,
            UploadedByName = string.Empty,
            UploadedAt = doc.UploadedAt
        };
    }

    public async Task<bool> DeleteDocumentAsync(int documentId)
    {
        var doc = await _context.OnboardingDocuments.FindAsync(documentId);
        if (doc == null) return false;
        var onboarding = await _context.Onboardings.FindAsync(doc.OnboardingId);
        if (onboarding != null && onboarding.DocumentsCount > 0) onboarding.DocumentsCount--;
        _context.OnboardingDocuments.Remove(doc);
        await _context.SaveChangesAsync();
        return true;
    }
}
