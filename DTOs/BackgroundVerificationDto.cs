using System.ComponentModel.DataAnnotations;

namespace RecruitmentSystem.API.DTOs;

public class BackgroundVerificationDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public int CandidateId { get; set; }
    public int JobId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public List<string> Stages { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public string Risk { get; set; } = string.Empty;
    public int ChecksRun { get; set; }
    public int IssuesFound { get; set; }
    public string? StartedAt { get; set; }
    public string? CompletedAt { get; set; }
    public int TurnaroundDays { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<BackgroundVerificationDocumentDto> Documents { get; set; } = new();
}

public class BackgroundVerificationDocumentDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string Comments { get; set; } = string.Empty;
    public int? UploadedById { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class CreateBackgroundVerificationDto
{
    [Required] public int CandidateId { get; set; }
    [Required] public int JobId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public List<string> Stages { get; set; } = new();
    public List<UploadBackgroundDocumentDto> Documents { get; set; } = new();
}

public class UploadBackgroundDocumentDto
{
    [Required] public string FileName { get; set; } = string.Empty;
    [Required] public string Url { get; set; } = string.Empty; // client uploads to storage and passes URL
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string? Comments { get; set; }
}
