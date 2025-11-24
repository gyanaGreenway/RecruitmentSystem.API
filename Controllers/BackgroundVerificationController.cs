using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;
using System.Security.Claims;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/[controller]")]
[Authorize(Roles = "HR,Recruiter")]
public class BackgroundVerificationController : ControllerBase
{
    private readonly IBackgroundVerificationService _service;

    public BackgroundVerificationController(IBackgroundVerificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<BackgroundVerificationDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25, [FromQuery] string? status = null)
    {
        var result = await _service.GetAllAsync(pageNumber, pageSize, status);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BackgroundVerificationDto>> Get(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<BackgroundVerificationDto>> Create([FromBody] CreateBackgroundVerificationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var createdBy = int.TryParse(userIdClaim, out var uid) ? uid : 0;
        var created = await _service.CreateAsync(dto, createdBy);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var ok = await _service.CancelAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/status")]
    public async Task<ActionResult<BackgroundVerificationDto>> UpdateStatus(int id, [FromQuery] string status, [FromBody] string? notes = null)
    {
        var updated = await _service.UpdateStatusAsync(id, status, notes);
        if (updated == null) return NotFound();
        return Ok(updated);
    }
}
