namespace RecruitmentSystem.API.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public int? JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int MatchPercentage { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool Read { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ActionUrl { get; set; }
}

public class CreateNotificationDto
{
    public int CandidateId { get; set; }
    public int? JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int MatchPercentage { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
}
