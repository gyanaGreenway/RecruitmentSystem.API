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

