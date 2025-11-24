using System.ComponentModel.DataAnnotations;

namespace RecruitmentSystem.API.DTOs;

public class InterviewCalendarDto
{
    public int InterviewId { get; set; }
    public Guid PublicId { get; set; }
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public int? InterviewerId { get; set; }
    public string InterviewerName { get; set; } = string.Empty;
    public string Start { get; set; } = string.Empty; // ISO 8601
    public int DurationMinutes { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? MeetingLink { get; set; }
    public string Type { get; set; } = string.Empty; // virtual | onsite | in_person
}

public class InterviewFeedbackDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public int InterviewerId { get; set; }
    public string InterviewerName { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty; // ISO
    public int Score { get; set; }
    public string Verdict { get; set; } = string.Empty;
    public List<string> Strengths { get; set; } = new();
    public List<string> Reservations { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string NextActions { get; set; } = string.Empty;
}

public class ScheduleInterviewRequest
{
    [Required] public int CandidateId { get; set; }
    [Required] public int JobId { get; set; }
    [Required] public string Stage { get; set; } = string.Empty;
    [Required] public List<int> InterviewerIds { get; set; } = new();
    [Required] public DateTime StartUtc { get; set; }
    [Required] public int DurationMinutes { get; set; }
    [Required] public string Timezone { get; set; } = string.Empty;
    [Required] public string LocationType { get; set; } = "virtual";
    public string MeetingProvider { get; set; } = string.Empty;
    public string? MeetingLink { get; set; }
    public string? LocationDetail { get; set; }
    public string Notes { get; set; } = string.Empty;
    public List<string> Reminders { get; set; } = new();
    public List<string> Flags { get; set; } = new();
}

public class InterviewCalendarResponse
{
    public List<InterviewCalendarDto> Events { get; set; } = new();
    public int TotalCount { get; set; }
    public string GeneratedAt { get; set; } = string.Empty;
}

public class InterviewMetadataResponse
{
    public List<string> Stage { get; set; } = new();
    public List<SimpleItemDto> Interviewer { get; set; } = new();
    public List<string> Timezone { get; set; } = new();
    public List<string> Reminder { get; set; } = new();
    public List<SimpleItemDto> Location { get; set; } = new();
    public List<SuggestedSlotDto> SuggestedSlots { get; set; } = new();
    public string DefaultTimezone { get; set; } = "UTC";
    public List<string> MeetingProviders { get; set; } = new();
    public string DefaultProvider { get; set; } = string.Empty;
}

public class SimpleItemDto { public string Id { get; set; } = string.Empty; public string Label { get; set; } = string.Empty; }
public class SuggestedSlotDto { public string Id { get; set; } = string.Empty; public string Label { get; set; } = string.Empty; public string Start { get; set; } = string.Empty; public string Timezone { get; set; } = string.Empty; public int DurationMinutes { get; set; } }

public class InterviewFeedbackFilter
{
    public string? Status { get; set; }
    public string? Verdict { get; set; }
    public int? JobId { get; set; }
    public int? InterviewerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
