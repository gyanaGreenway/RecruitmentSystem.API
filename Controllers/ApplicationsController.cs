using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;
using RecruitmentSystem.API.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

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

    [HttpPost("public")]
    [AllowAnonymous]
    public async Task<ActionResult<ApplicationDto>> PublicApply([FromBody] PublicApplicationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Basic input sanitation (trim)
        dto.Email = dto.Email.Trim();
        dto.FirstName = dto.FirstName.Trim();
        dto.LastName = dto.LastName.Trim();
        dto.Phone = dto.Phone.Trim();
        dto.KeySkills = dto.KeySkills?.Trim();
        dto.ResumeUrl = dto.ResumeUrl?.Trim();
        dto.Notes = dto.Notes?.Trim();

        // Email format already validated by DataAnnotations; extra regex hardening optional
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        if (!emailRegex.IsMatch(dto.Email)) return BadRequest(new { message = "Invalid email format." });

        // Phone validation (already by regex attribute) – re-check
        if (!Regex.IsMatch(dto.Phone, @"^[+]?\d{7,20}$")) return BadRequest(new { message = "Invalid phone format." });

        // Job existence
        var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == dto.JobId && j.IsActive);
        if (job == null) return NotFound(new { message = "Job not found or inactive." });

        // Start transaction
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            // Check if candidate exists
            var existingCandidate = await _db.Candidates.FirstOrDefaultAsync(c => c.Email == dto.Email);
            int candidateId;
            string passwordHash;
            if (existingCandidate == null)
            {
                // Ensure no user exists with same email
                var existingUser = await _db.Users.AnyAsync(u => u.Email == dto.Email);
                if (existingUser) return Conflict(new { message = "Email already registered." });

                passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 11);
                var candidate = new Candidate
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    ResumeUrl = dto.ResumeUrl ?? string.Empty,
                    PasswordHash = passwordHash,
                    KeySkills = dto.KeySkills ?? string.Empty
                };
                _db.Candidates.Add(candidate);
                await _db.SaveChangesAsync();
                candidateId = candidate.Id;

                // Create user record
                var user = new User
                {
                    Email = dto.Email,
                    PasswordHash = passwordHash,
                    Role = UserRole.Candidate
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
            else
            {
                candidateId = existingCandidate.Id;
            }

            // Prevent duplicate application for same job unless previous was rejected
            var existingApp = await _db.Applications.FirstOrDefaultAsync(a => a.JobId == dto.JobId && a.CandidateId == candidateId);
            if (existingApp != null)
            {
                if (existingApp.Status == ApplicationStatus.Rejected)
                {
                    existingApp.Status = ApplicationStatus.New;
                    existingApp.AppliedDate = DateTime.UtcNow;
                    existingApp.Notes = dto.Notes;
                    await _db.SaveChangesAsync();
                    _db.ApplicationStatusHistories.Add(new ApplicationStatusHistory
                    {
                        ApplicationId = existingApp.Id,
                        PreviousStatus = ApplicationStatus.Rejected,
                        NewStatus = ApplicationStatus.New,
                        ChangedAt = DateTime.UtcNow,
                        Notes = "Candidate reapplied (public)"
                    });
                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();
                    var reopened = await _applicationService.GetApplicationByIdAsync(existingApp.Id);
                    return CreatedAtAction(nameof(GetApplication), new { id = existingApp.Id }, reopened);
                }
                await tx.RollbackAsync();
                return Conflict(new { message = "Application already exists for this job." });
            }

            var application = new Application
            {
                JobId = dto.JobId,
                CandidateId = candidateId,
                Cycle = job.Cycle,
                Status = ApplicationStatus.New,
                AppliedDate = DateTime.UtcNow,
                Notes = dto.Notes
            };
            _db.Applications.Add(application);
            await _db.SaveChangesAsync();

            _db.ApplicationStatusHistories.Add(new ApplicationStatusHistory
            {
                ApplicationId = application.Id,
                PreviousStatus = ApplicationStatus.New,
                NewStatus = ApplicationStatus.New,
                ChangedAt = DateTime.UtcNow,
                Notes = "Application created (public)"
            });
            await _db.SaveChangesAsync();

            await tx.CommitAsync();
            var created = await _applicationService.GetApplicationByIdAsync(application.Id);
            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, created);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return StatusCode(500, new { message = "Failed to submit application.", detail = ex.Message });
        }
    }
}

