using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;
using System.Linq.Expressions;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/public/[controller]")]
public class PublicJobsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PublicJobsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Reusable projection (translatable by EF)
    private static Expression<Func<Job, JobDto>> JobSelector => j => new JobDto
    {
        Id = j.Id,
        PublicId = j.PublicId,
        Title = j.Title,
        Description = j.Description,
        Department = j.Department,
        Location = j.Location,
        Salary = j.Salary,
        PostedDate = j.PostedDate,
        ClosingDate = j.ClosingDate,
        IsActive = j.IsActive
    };

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<JobDto>>> GetActiveJobs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = _context.Jobs.Where(j => j.IsActive);
        var totalCount = await query.CountAsync();

        var jobs = await query
            .OrderByDescending(j => j.PostedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(JobSelector)
            .ToListAsync();

        return Ok(new PagedResultDto<JobDto>
        {
            Items = jobs,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    // Find by numeric Id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobDto>> GetJob(int id)
    {
        var job = await _context.Jobs
            .Where(j => j.Id == id && j.IsActive)
            .Select(JobSelector)
            .FirstOrDefaultAsync();

        return job == null ? NotFound() : Ok(job);
    }

    // Find by PublicId (GUID)
    [HttpGet("{publicId:guid}")]
    public async Task<ActionResult<JobDto>> GetJobByPublicId(Guid publicId)
    {
        var job = await _context.Jobs
            .Where(j => j.PublicId == publicId && j.IsActive)
            .Select(JobSelector)
            .FirstOrDefaultAsync();

        return job == null ? NotFound() : Ok(job);
    }
}

