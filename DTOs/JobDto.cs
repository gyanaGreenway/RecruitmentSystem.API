using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RecruitmentSystem.API.DTOs;

public class JobDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateJobDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Salary { get; set; }

    public DateTime? ClosingDate { get; set; }

    // Requirements as list of skill strings
    public List<string>? Requirements { get; set; }

    // Whether the job is active
    public bool IsActive { get; set; } = true;
}

public class UpdateJobDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Salary { get; set; }

    public DateTime? ClosingDate { get; set; }

    public List<string>? Requirements { get; set; }

    public bool IsActive { get; set; } = true;
}

