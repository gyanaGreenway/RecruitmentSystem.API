using System.ComponentModel.DataAnnotations;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.DTOs;

public class OfferLetterDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public int? RecruiterId { get; set; }
    public string RecruiterName { get; set; } = string.Empty;
    public DateTime? SentOn { get; set; }
    public DateTime? TargetStart { get; set; }
    public DateTime? LastTouched { get; set; }
    public string Compensation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Attachments { get; set; }
    public string Notes { get; set; } = string.Empty;
    public double? AcceptanceProbability { get; set; }
    public string? OfferLink { get; set; }
}

public class CreateOfferLetterDto
{
    [Required] public int CandidateId { get; set; }
    [Required] public int JobId { get; set; }
    public int? RecruiterId { get; set; }
    public string Compensation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Attachments { get; set; }
    public string Notes { get; set; } = string.Empty;
    public double? AcceptanceProbability { get; set; }
    public DateTime? TargetStart { get; set; }
    public string? OfferLink { get; set; }
}

public class UpdateOfferLetterDto
{
    [Required] public string Status { get; set; } = string.Empty;
    public string Compensation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Attachments { get; set; }
    public string Notes { get; set; } = string.Empty;
    public double? AcceptanceProbability { get; set; }
    public DateTime? TargetStart { get; set; }
    public string? OfferLink { get; set; }
}

public class OfferFilterDto
{
    public string? Status { get; set; }
    public int? RecruiterId { get; set; }
    public int? CandidateId { get; set; }
    public int? JobId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public string? SortBy { get; set; } = "SentOn";
    public string? SortOrder { get; set; } = "desc";
}

public class PagedResultOfferDto
{
    public List<OfferLetterDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
