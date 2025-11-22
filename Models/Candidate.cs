namespace RecruitmentSystem.API.Models;

public class Candidate
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string ResumeHeadline { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string KeySkills { get; set; } = string.Empty; // comma-separated skills
    public string ProfileSummary { get; set; } = string.Empty;
    public string Accomplishments { get; set; } = string.Empty;
    public string CareerProfile { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; }

    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<CandidateEmployment> Employment { get; set; } = new List<CandidateEmployment>();
    public ICollection<CandidateEducation> Education { get; set; } = new List<CandidateEducation>();
    public ICollection<CandidateSkill> ITSkills { get; set; } = new List<CandidateSkill>();
    public ICollection<CandidateProject> Projects { get; set; } = new List<CandidateProject>();
}

public class CandidateEmployment
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string WorkArea { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool CurrentlyWorking { get; set; }
    public string Description { get; set; } = string.Empty;
    public Candidate Candidate { get; set; } = null!;
}

public class CandidateEducation
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Candidate Candidate { get; set; } = null!;
}

public class CandidateSkill
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public string Skill { get; set; } = string.Empty;
    public string Proficiency { get; set; } = string.Empty; // e.g. Beginner/Intermediate/Advanced/Expert
    public Candidate Candidate { get; set; } = null!;
}

public class CandidateProject
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Candidate Candidate { get; set; } = null!;
}

