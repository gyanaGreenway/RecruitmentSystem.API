namespace RecruitmentSystem.API.Models;

public class Application
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int CandidateId { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.New;
    public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public Job Job { get; set; } = null!;
    public Candidate Candidate { get; set; } = null!;
    public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
}

public enum ApplicationStatus
{
    New = 1,
    Shortlisted = 2,
    Rejected = 3,
    Hired = 4
}

