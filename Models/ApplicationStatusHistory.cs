namespace RecruitmentSystem.API.Models;

public class ApplicationStatusHistory
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public ApplicationStatus PreviousStatus { get; set; }
    public ApplicationStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? ChangedBy { get; set; }
    public string? Notes { get; set; }

    public Application Application { get; set; } = null!;
}

