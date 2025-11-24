using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/[controller]")]
[Authorize(Roles = "HR,Candidate")] // adjust as needed
public class InterviewsController : ControllerBase
{
    private readonly IInterviewService _service;

    public InterviewsController(IInterviewService service)
    {
        _service = service;
    }

    [HttpGet("calendar")]
    public async Task<ActionResult<InterviewCalendarResponse>> GetCalendar([FromQuery] int? jobId, [FromQuery] string? stage, [FromQuery] int? interviewerId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var resp = await _service.GetCalendarAsync(jobId, stage, interviewerId, startDate, endDate);
        return Ok(resp);
    }

    [HttpGet("feedback")]
    public async Task<ActionResult<List<InterviewFeedbackDto>>> GetFeedback([FromQuery] InterviewFeedbackFilter filter)
    {
        var list = await _service.GetFeedbackAsync(filter);
        return Ok(list);
    }

    [HttpGet("schedule/metadata")]
    public async Task<ActionResult<InterviewMetadataResponse>> GetMetadata()
    {
        var data = await _service.GetMetadataAsync();
        return Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult<InterviewCalendarDto>> Schedule([FromBody] ScheduleInterviewRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.ScheduleAsync(request);
        return CreatedAtAction(nameof(GetCalendar), new { jobId = created.JobId }, created);
    }
}
