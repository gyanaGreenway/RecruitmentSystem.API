using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;
using System.Security.Claims;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/[controller]")]
[Authorize(Roles = "HR,Candidate")] // allow both, tighten per action
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidatesController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [AllowAnonymous]
    [HttpPost("register/start")] // send OTP to email, returns request id
    public async Task<ActionResult<object>> StartRegistration([FromBody] StartCandidateRegistrationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var requestId = await _candidateService.StartCandidateRegistrationAsync(dto);
            return Ok(new { requestId });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("register/confirm")] // verify OTP and create account
    public async Task<ActionResult<CandidateDto>> ConfirmRegistration([FromBody] ConfirmCandidateRegistrationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var candidate = await _candidateService.ConfirmCandidateRegistrationAsync(dto);
            return Ok(candidate);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "HR")] // list only HR
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CandidateDto>>> GetCandidates([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _candidateService.GetAllCandidatesAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")] // candidate can only fetch self
    public async Task<ActionResult<CandidateDto>> GetCandidate(int id)
    {
        var candidate = await _candidateService.GetCandidateByIdAsync(id);
        if (candidate == null) return NotFound();

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, "Candidate", StringComparison.OrdinalIgnoreCase))
        {
            var claimId = User.FindFirst("candidate_id")?.Value;
            if (int.TryParse(claimId, out var tokenCid))
            {
                if (tokenCid != id) return Forbid();
            }
            else
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (!string.Equals(candidate.Email, email, StringComparison.OrdinalIgnoreCase)) return Forbid();
            }
        }
        return Ok(candidate);
    }

    [Authorize(Roles = "HR")] // creation only HR (candidates use public registration)
    [HttpPost]
    public async Task<ActionResult<CandidateDto>> CreateCandidate([FromBody] CreateCandidateDto createCandidateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var candidate = await _candidateService.CreateCandidateAsync(createCandidateDto);
        return CreatedAtAction(nameof(GetCandidate), new { id = candidate.Id }, candidate);
    }

    [HttpPut("{id}")] // candidate can update self; HR can update any
    public async Task<ActionResult<CandidateDto>> UpdateCandidate(int id, [FromBody] UpdateCandidateDto updateCandidateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, "Candidate", StringComparison.OrdinalIgnoreCase))
        {
            var claimId = User.FindFirst("candidate_id")?.Value;
            if (int.TryParse(claimId, out var tokenCid))
            {
                if (tokenCid != id) return Forbid();
            }
            else
            {
                var current = await _candidateService.GetCandidateByIdAsync(id);
                if (current == null) return NotFound();
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (!string.Equals(current.Email, email, StringComparison.OrdinalIgnoreCase)) return Forbid();
            }
        }

        var candidate = await _candidateService.UpdateCandidateAsync(id, updateCandidateDto);
        if (candidate == null) return NotFound();
        return Ok(candidate);
    }

    [Authorize(Roles = "HR")] // only HR can delete (soft delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCandidate(int id)
    {
        var result = await _candidateService.DeleteCandidateAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}

