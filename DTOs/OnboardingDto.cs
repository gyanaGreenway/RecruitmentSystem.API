using System.ComponentModel.DataAnnotations;

namespace RecruitmentSystem.API.DTOs;

public class OnboardingDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public int? RecruiterId { get; set; }
    public string RecruiterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int TasksCount { get; set; }
    public int CompletedTasksCount { get; set; }
    public int DocumentsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OnboardingTaskDto> Tasks { get; set; } = new();
    public List<OnboardingDocumentDto> Documents { get; set; } = new();
}

public class OnboardingTaskDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? AssignedToId { get; set; }
    public string AssignedToName { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pending | Completed | Overdue
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OnboardingDocumentDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string Status { get; set; } = string.Empty; // Pending | Approved | Rejected
    public string Comments { get; set; } = string.Empty;
    public int? UploadedById { get; set; }
    public string UploadedByName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class CreateOnboardingDto
{
    [Required] public int CandidateId { get; set; }
    [Required] public int JobId { get; set; }
    public int? RecruiterId { get; set; }
    public DateTime? DueDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public List<CreateOnboardingTaskDto> Tasks { get; set; } = new();
}

public class CreateOnboardingTaskDto
{
    [Required, StringLength(256)] public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? AssignedToId { get; set; }
    public DateTime? DueDate { get; set; }
}

public class UpdateOnboardingDto
{
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class UpdateOnboardingTaskDto
{
    [Required, StringLength(256)] public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? AssignedToId { get; set; }
    public DateTime? DueDate { get; set; }
    public bool Completed { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class UploadOnboardingDocumentDto
{
    [Required, StringLength(256)] public string FileName { get; set; } = string.Empty;
    [Required, StringLength(512)] public string Url { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string? Comments { get; set; }
}

public class OnboardingFilterDto
{
    public int? CandidateId { get; set; }
    public int? JobId { get; set; }
    public string? Status { get; set; }
    public int? RecruiterId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public class PagedOnboardingResult
{
    public List<OnboardingDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
