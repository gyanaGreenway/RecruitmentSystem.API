using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;
using System.Security.Claims;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/[controller]")]
[Authorize(Roles = "HR,Candidate")]
public class OnboardingController : ControllerBase
{
    private readonly IOnboardingService _service;

    public OnboardingController(IOnboardingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedOnboardingResult>> GetAll([FromQuery] OnboardingFilterDto filter)
    {
        var result = await _service.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OnboardingDto>> GetById(int id)
    {
        var onboarding = await _service.GetByIdAsync(id);
        if (onboarding == null) return NotFound();
        return Ok(onboarding);
    }

    [Authorize(Roles = "HR")]
    [HttpPost]
    public async Task<ActionResult<OnboardingDto>> Create([FromBody] CreateOnboardingDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "HR")]
    [HttpPut("{id}")]
    public async Task<ActionResult<OnboardingDto>> Update(int id, [FromBody] UpdateOnboardingDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "HR")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = "HR")]
    [HttpPost("{onboardingId}/tasks")]
    public async Task<ActionResult<OnboardingTaskDto>> AddTask(int onboardingId, [FromBody] CreateOnboardingTaskDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var task = await _service.AddTaskAsync(onboardingId, dto);
        return Ok(task);
    }

    [HttpPut("tasks/{taskId}")]
    public async Task<ActionResult<OnboardingTaskDto>> UpdateTask(int taskId, [FromBody] UpdateOnboardingTaskDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var task = await _service.UpdateTaskAsync(taskId, dto);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [Authorize(Roles = "HR")]
    [HttpDelete("tasks/{taskId}")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        var ok = await _service.DeleteTaskAsync(taskId);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpPost("{onboardingId}/documents")]
    public async Task<ActionResult<OnboardingDocumentDto>> UploadDocument(int onboardingId, [FromBody] UploadOnboardingDocumentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();
        var doc = await _service.UploadDocumentAsync(onboardingId, dto, userId);
        return Ok(doc);
    }

    [Authorize(Roles = "HR")]
    [HttpDelete("documents/{documentId}")]
    public async Task<IActionResult> DeleteDocument(int documentId)
    {
        var ok = await _service.DeleteDocumentAsync(documentId);
        if (!ok) return NotFound();
        return NoContent();
    }
}
