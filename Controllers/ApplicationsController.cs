using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;
using System.Security.Claims;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ApplicationDbContext _db;

    public ApplicationsController(IApplicationService applicationService, ApplicationDbContext db)
    {
        _applicationService = applicationService;
        _db = db;
    }

    [HttpPost]
    [Authorize(Roles = "Candidate,HR")] // allow HR to create applications too
    public async Task<ActionResult<ApplicationDto>> CreateApplication([FromBody] CreateApplicationDto createApplicationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Enforce candidate identity consistency
        var role = User.FindFirstValue(ClaimTypes.Role);
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
        if (string.Equals(role, "Candidate", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(new { message = "Unable to resolve current user email from token." });
            }

            var candidate = await _db.Candidates.FirstOrDefaultAsync(c => c.Email == email);
            if (candidate == null)
            {
                return BadRequest(new { message = "Candidate profile not found for the logged-in user." });
            }

            // Ignore any provided CandidateId and use the current user's candidate id
            createApplicationDto.CandidateId = candidate.Id;
        }
        else if (string.Equals(role, "HR", StringComparison.OrdinalIgnoreCase))
        {
            if (createApplicationDto.CandidateId <= 0)
            {
                return BadRequest(new { message = "CandidateId is required when creating an application as HR." });
            }
        }

        try
        {
            var application = await _applicationService.CreateApplicationAsync(createApplicationDto);
            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }
        catch (InvalidOperationException ex)
        {
            // Service throws InvalidOperationException for duplicates or invalid ops
            return Conflict(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            // Database-level uniqueness violation or other DB error
            return Conflict(new { message = "Application already exists for this job and candidate." });
        }
    }

    [HttpGet]
    [Authorize(Roles = "HR,Candidate")]
    public async Task<ActionResult<PagedResultDto<ApplicationDto>>> GetApplications([FromQuery] ApplicationFilterDto filter)
    {
        var result = await _applicationService.GetApplicationsAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationDto>> GetApplication(int id)
    {
        var application = await _applicationService.GetApplicationByIdAsync(id);
        if (application == null) return NotFound();
        return Ok(application);
    }

    [HttpGet("{id}/history")]
    [Authorize(Roles = "HR,Candidate")]
    public async Task<ActionResult<List<ApplicationStatusHistoryDto>>> GetHistory(int id)
    {
        var history = await _applicationService.GetStatusHistoryAsync(id);
        return Ok(history);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "HR")] // only HR/Admin updates status
    public async Task<ActionResult<ApplicationDto>> UpdateStatus(int id, [FromBody] UpdateApplicationStatusDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Use email as ChangedBy (preferred). Fallback to userId if email unavailable.
            string? changedBy = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(changedBy))
            {
                changedBy = User?.FindFirstValue(ClaimTypes.Email) ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var updated = await _applicationService.UpdateApplicationStatusAsync(id, updateDto, changedBy);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

