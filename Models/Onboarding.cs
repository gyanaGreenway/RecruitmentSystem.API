namespace RecruitmentSystem.API.Models;

public enum OnboardingStatus { Pending = 0, InProgress = 1, Completed = 2, Cancelled = 3 }
public enum OnboardingTaskStatus { Pending = 0, Completed = 1, Overdue = 2 }
public enum OnboardingDocumentStatus { Pending = 0, Approved = 1, Rejected = 2 }

public class Onboarding
{
    public int Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public int CandidateId { get; set; }
    public int JobId { get; set; }
    public int? RecruiterId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public OnboardingStatus Status { get; set; } = OnboardingStatus.Pending;
    public string Notes { get; set; } = string.Empty;
    public int TasksCount { get; set; }
    public int DocumentsCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<OnboardingTask> Tasks { get; set; } = new List<OnboardingTask>();
    public ICollection<OnboardingDocument> Documents { get; set; } = new List<OnboardingDocument>();
}

public class OnboardingTask
{
    public int Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public int OnboardingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? AssignedToId { get; set; }
    public DateTime? DueDate { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
    public OnboardingTaskStatus Status { get; set; } = OnboardingTaskStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class OnboardingDocument
{
    public int Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public int OnboardingId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public OnboardingDocumentStatus Status { get; set; } = OnboardingDocumentStatus.Pending;
    public string Comments { get; set; } = string.Empty;
    public int? UploadedById { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
