namespace RecruitmentSystem.API.Models;

public enum BgStatus { Pending = 0, InProgress = 1, Review = 2, Cleared = 3, Exceptions = 4 }
public enum BgRisk { Low = 0, Medium = 1, High = 2 }

public class BackgroundVerification
{
    public int Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public int CandidateId { get; set; }
    public int JobId { get; set; }
    public string Provider { get; set; } = string.Empty; // external provider name
    public string Stages { get; set; } = string.Empty; // comma-separated stages like "Employment,Education,Criminal"
    public BgStatus Status { get; set; } = BgStatus.Pending;
    public BgRisk Risk { get; set; } = BgRisk.Low;
    public int ChecksRun { get; set; }
    public int IssuesFound { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TurnaroundDays { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty; // provider's id
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<BackgroundVerificationDocument> Documents { get; set; } = new List<BackgroundVerificationDocument>();
}

public class BackgroundVerificationDocument
{
    public int Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public int BackgroundVerificationId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty; // encrypted by EncryptionService before save
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string Comments { get; set; } = string.Empty;
    public int? UploadedById { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
