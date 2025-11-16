using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("candidate/{candidateId}")]
    [Authorize(Roles = "Candidate,HR")]
    public async Task<ActionResult<List<NotificationDto>>> GetForCandidate(int candidateId)
    {
        var list = await _notificationService.GetNotificationsForCandidateAsync(candidateId);
        return Ok(list);
    }

    [HttpGet("candidate/{candidateId}/unread-count")]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<int>> GetUnreadCount(int candidateId)
    {
        var count = await _notificationService.GetUnreadCountAsync(candidateId);
        return Ok(count);
    }

    [HttpPut("{id}/read")]
    [Authorize(Roles = "Candidate,HR")]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok();
    }

    [HttpPut("candidate/{candidateId}/mark-all-read")]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> MarkAllRead(int candidateId)
    {
        await _notificationService.MarkAllReadAsync(candidateId);
        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "HR")]
    public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationDto createDto)
    {
        var created = await _notificationService.CreateNotificationAsync(createDto);
        return CreatedAtAction(nameof(GetForCandidate), new { candidateId = created.CandidateId }, created);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "HR,Candidate")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _notificationService.DeleteNotificationAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}
