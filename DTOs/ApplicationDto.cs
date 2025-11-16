using System.ComponentModel.DataAnnotations;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.DTOs;

public class ApplicationDto
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int CandidateId { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime AppliedDate { get; set; }
    public string? Notes { get; set; }
    public JobDto? Job { get; set; }
    public CandidateDto? Candidate { get; set; }
}

public class CreateApplicationDto
{
    [Required]
    public int JobId { get; set; }

    [Required]
    public int CandidateId { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

public class UpdateApplicationStatusDto
{
    [Required]
    public ApplicationStatus Status { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

public class ApplicationFilterDto
{
    public ApplicationStatus? Status { get; set; }
    public int? JobId { get; set; }
    public int? CandidateId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "AppliedDate";
    public string? SortOrder { get; set; } = "desc";
}

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

