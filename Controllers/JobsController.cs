using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/[controller]")]
[Authorize(Roles = "HR")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<JobDto>>> GetJobs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _jobService.GetAllJobsAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobDto>> GetJob(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpPost]
    public async Task<ActionResult<JobDto>> CreateJob([FromBody] CreateJobDto createJobDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var job = await _jobService.CreateJobAsync(createJobDto);
        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<JobDto>> UpdateJob(int id, [FromBody] UpdateJobDto updateJobDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var job = await _jobService.UpdateJobAsync(id, updateJobDto);
        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var result = await _jobService.DeleteJobAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}

