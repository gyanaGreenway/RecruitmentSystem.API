namespace RecruitmentSystem.API.Models;

public class Notification
{
 public int Id { get; set; }
 public int CandidateId { get; set; }
 public int? JobId { get; set; }
 public string JobTitle { get; set; } = string.Empty;
 public string Message { get; set; } = string.Empty;
 public int MatchPercentage { get; set; }
 public string Type { get; set; } = string.Empty;
 public bool Read { get; set; } = false;
 public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
 public string? ActionUrl { get; set; }

 public Candidate? Candidate { get; set; }
}
