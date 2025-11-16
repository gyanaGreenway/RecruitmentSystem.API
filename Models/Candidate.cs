namespace RecruitmentSystem.API.Models;

public class Candidate
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string KeySkills { get; set; } = string.Empty; // comma-separated skills
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

