namespace RecruitmentSystem.API.Models;

public enum InterviewLocationType { Virtual = 0, Onsite = 1, InPerson = 2 }

public class Interview
{
    public int Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public int CandidateId { get; set; }
    public int JobId { get; set; }
    public string Stage { get; set; } = string.Empty;
    public DateTime StartUtc { get; set; }
    public int DurationMinutes { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public InterviewLocationType LocationType { get; set; } = InterviewLocationType.Virtual;
    public string MeetingProvider { get; set; } = string.Empty;
    public string? MeetingLink { get; set; }
    public string? LocationDetail { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Reminders { get; set; } = string.Empty;
    public string Flags { get; set; } = string.Empty;
    public string InterviewerIds { get; set; } = string.Empty;
    public string InterviewerNames { get; set; } = string.Empty;
    public string ExternalEventId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class InterviewFeedback
{
    public int Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public int InterviewId { get; set; }
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public int InterviewerId { get; set; }
    public string InterviewerName { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public int Score { get; set; }
    public string Verdict { get; set; } = string.Empty;
    public string Strengths { get; set; } = string.Empty;
    public string Reservations { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string NextActions { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
