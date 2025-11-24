namespace RecruitmentSystem.API.Models;

public enum OfferStatus
{
    Draft = 0,
    Pending = 1,
    Negotiation = 2,
    Accepted = 3,
    Declined = 4,
    Withdrawn = 5,
    Expired = 6
}

public class OfferLetter
{
    public int Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public OfferStatus Status { get; set; } = OfferStatus.Draft;
    public int CandidateId { get; set; }
    public int JobId { get; set; }
    public int? RecruiterId { get; set; }

    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string RecruiterName { get; set; } = string.Empty;

    public DateTime? SentOn { get; set; }
    public DateTime? TargetStart { get; set; }
    public DateTime? LastTouched { get; set; }

    public string Compensation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int AttachmentsCount { get; set; }
    public string Notes { get; set; } = string.Empty;
    public double? AcceptanceProbability { get; set; }
    public string? OfferLink { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Candidate? Candidate { get; set; }
    public Job? Job { get; set; }
    public User? Recruiter { get; set; }
}
