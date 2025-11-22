using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RecruitmentSystem.API.DTOs;

public class CandidateDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? ResumeUrl { get; set; }
    public string ResumeHeadline { get; set; } = string.Empty;
    public List<string> KeySkills { get; set; } = new();
    public List<CandidateEmploymentItemDto> Employment { get; set; } = new();
    public List<CandidateEducationItemDto> Education { get; set; } = new();
    public List<CandidateSkillItemDto> ITSkills { get; set; } = new();
    public List<CandidateProjectItemDto> Projects { get; set; } = new();
    public string ProfileSummary { get; set; } = string.Empty;
    public string Accomplishments { get; set; } = string.Empty;
    public string CareerProfile { get; set; } = string.Empty;
    public CandidatePersonalDetailsDto? PersonalDetails { get; set; }
    public bool IsDeleted { get; set; }
    public string? RowVersion { get; set; } // base64
}

public class CandidateEmploymentItemDto
{
    public int Id { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string WorkArea { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool CurrentlyWorking { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CandidateEducationItemDto
{
    public int Id { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CandidateSkillItemDto
{
    public int Id { get; set; }
    public string Skill { get; set; } = string.Empty;
    public string Proficiency { get; set; } = string.Empty;
}

public class CandidateProjectItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CandidatePersonalDetailsDto
{
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class CreateCandidateDto
{
    [Required, StringLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(100)] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(200)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(20)] public string Phone { get; set; } = string.Empty;
    [StringLength(500)] public string? ResumeUrl { get; set; }
    [Required, StringLength(100, MinimumLength = 6)] public string Password { get; set; } = string.Empty;
    [StringLength(200)] public string ResumeHeadline { get; set; } = string.Empty;
    public List<string> KeySkills { get; set; } = new();
    public string ProfileSummary { get; set; } = string.Empty;
    public string Accomplishments { get; set; } = string.Empty;
    public string CareerProfile { get; set; } = string.Empty;
    public CandidatePersonalDetailsDto? PersonalDetails { get; set; }
    public List<CandidateEmploymentItemDto> Employment { get; set; } = new();
    public List<CandidateEducationItemDto> Education { get; set; } = new();
    public List<CandidateSkillItemDto> ITSkills { get; set; } = new();
    public List<CandidateProjectItemDto> Projects { get; set; } = new();
}

public class UpdateCandidateDto
{
    [Required, StringLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(100)] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(200)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(20)] public string Phone { get; set; } = string.Empty;
    [StringLength(500)] public string? ResumeUrl { get; set; }
    [StringLength(200)] public string ResumeHeadline { get; set; } = string.Empty;
    public List<string> KeySkills { get; set; } = new();
    public string ProfileSummary { get; set; } = string.Empty;
    public string Accomplishments { get; set; } = string.Empty;
    public string CareerProfile { get; set; } = string.Empty;
    public CandidatePersonalDetailsDto? PersonalDetails { get; set; }
    public List<CandidateEmploymentItemDto> Employment { get; set; } = new();
    public List<CandidateEducationItemDto> Education { get; set; } = new();
    public List<CandidateSkillItemDto> ITSkills { get; set; } = new();
    public List<CandidateProjectItemDto> Projects { get; set; } = new();
    [Required] public string RowVersion { get; set; } = string.Empty; // for concurrency
}

public class StartCandidateRegistrationDto
{
    [Required, EmailAddress, StringLength(200)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(20)] public string Phone { get; set; } = string.Empty;
    [Required, StringLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(100)] public string LastName { get; set; } = string.Empty;
    [StringLength(500)] public string? ResumeUrl { get; set; }
    [Required, StringLength(100, MinimumLength = 6)] public string Password { get; set; } = string.Empty;
    public string KeySkillsRaw { get; set; } = string.Empty; // preserve legacy
}

public class ConfirmCandidateRegistrationDto
{
    [Required] public int RequestId { get; set; }
    [Required, StringLength(8, MinimumLength = 4)] public string Otp { get; set; } = string.Empty;
}

