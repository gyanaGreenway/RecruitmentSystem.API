using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/[controller]")]
[Authorize(Roles = "HR")]
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

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CandidateDto>>> GetCandidates([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _candidateService.GetAllCandidatesAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CandidateDto>> GetCandidate(int id)
    {
        var candidate = await _candidateService.GetCandidateByIdAsync(id);
        if (candidate == null) return NotFound();
        return Ok(candidate);
    }

    [HttpPost]
    public async Task<ActionResult<CandidateDto>> CreateCandidate([FromBody] CreateCandidateDto createCandidateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var candidate = await _candidateService.CreateCandidateAsync(createCandidateDto);
        return CreatedAtAction(nameof(GetCandidate), new { id = candidate.Id }, candidate);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CandidateDto>> UpdateCandidate(int id, [FromBody] UpdateCandidateDto updateCandidateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var candidate = await _candidateService.UpdateCandidateAsync(id, updateCandidateDto);
        if (candidate == null) return NotFound();
        return Ok(candidate);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCandidate(int id)
    {
        var result = await _candidateService.DeleteCandidateAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}

