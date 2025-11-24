using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.Services;

public class InterviewService : IInterviewService
{
    private readonly ApplicationDbContext _context;
    private readonly IEnumerable<IMeetingProvider> _meetingProviders;

    public InterviewService(ApplicationDbContext context, IEnumerable<IMeetingProvider> meetingProviders)
    {
        _context = context;
        _meetingProviders = meetingProviders;
    }

    public async Task<InterviewCalendarResponse> GetCalendarAsync(int? jobId, string? stage, int? interviewerId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Interviews.AsQueryable();
        if (jobId.HasValue) query = query.Where(i => i.JobId == jobId.Value);
        if (!string.IsNullOrWhiteSpace(stage)) query = query.Where(i => i.Stage == stage);
        if (interviewerId.HasValue) query = query.Where(i => i.InterviewerIds.Contains($",{interviewerId.Value},") || i.InterviewerIds.StartsWith(interviewerId.Value.ToString()+",") || i.InterviewerIds.EndsWith(","+interviewerId.Value.ToString()) || i.InterviewerIds == interviewerId.Value.ToString());
        if (startDate.HasValue) query = query.Where(i => i.StartUtc >= startDate.Value);
        if (endDate.HasValue) query = query.Where(i => i.StartUtc <= endDate.Value);

        var list = await query.OrderBy(i => i.StartUtc).ToListAsync();
        var events = list.Select(i => new InterviewCalendarDto
        {
            InterviewId = i.Id,
            PublicId = i.PublicId,
            CandidateId = i.CandidateId,
            CandidateName = "", // populate later if needed
            JobId = i.JobId,
            JobTitle = "",
            Stage = i.Stage,
            InterviewerId = null, // multi interviewer condensed
            InterviewerName = string.Join(", ", i.InterviewerNames.Split(',', StringSplitOptions.RemoveEmptyEntries)),
            Start = i.StartUtc.ToString("o"),
            DurationMinutes = i.DurationMinutes,
            Timezone = i.Timezone,
            Label = $"{i.Stage} - {i.StartUtc:yyyy-MM-dd HH:mm}",
            MeetingLink = i.MeetingLink,
            Type = i.LocationType switch { InterviewLocationType.Virtual => "virtual", InterviewLocationType.Onsite => "onsite", InterviewLocationType.InPerson => "in_person", _ => "virtual" }
        }).ToList();
        return new InterviewCalendarResponse { Events = events, TotalCount = events.Count, GeneratedAt = DateTime.UtcNow.ToString("o") };
    }

    public async Task<List<InterviewFeedbackDto>> GetFeedbackAsync(InterviewFeedbackFilter filter)
    {
        var query = _context.InterviewFeedbacks.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Status)) query = query.Where(f => f.Status == filter.Status);
        if (!string.IsNullOrWhiteSpace(filter.Verdict)) query = query.Where(f => f.Verdict == filter.Verdict);
        if (filter.JobId.HasValue) query = query.Where(f => f.JobId == filter.JobId.Value);
        if (filter.InterviewerId.HasValue) query = query.Where(f => f.InterviewerId == filter.InterviewerId.Value);
        if (filter.StartDate.HasValue) query = query.Where(f => f.TimestampUtc >= filter.StartDate.Value);
        if (filter.EndDate.HasValue) query = query.Where(f => f.TimestampUtc <= filter.EndDate.Value);
        var list = await query.OrderByDescending(f => f.TimestampUtc).ToListAsync();
        return list.Select(f => new InterviewFeedbackDto
        {
            Id = f.Id,
            PublicId = f.PublicId,
            CandidateId = f.CandidateId,
            CandidateName = f.CandidateName,
            JobId = f.JobId,
            JobTitle = f.JobTitle,
            Stage = f.Stage,
            InterviewerId = f.InterviewerId,
            InterviewerName = f.InterviewerName,
            Timestamp = f.TimestampUtc.ToString("o"),
            Score = f.Score,
            Verdict = f.Verdict,
            Strengths = f.Strengths.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            Reservations = f.Reservations.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            Notes = f.Notes,
            Status = f.Status,
            NextActions = f.NextActions
        }).ToList();
    }

    public Task<InterviewMetadataResponse> GetMetadataAsync()
    {
        var resp = new InterviewMetadataResponse
        {
            Stage = new List<string> { "Screen", "Technical", "Manager", "HR", "Final" },
            Interviewer = new List<SimpleItemDto>(),
            Timezone = new List<string> { "UTC", "Asia/Kolkata", "America/New_York" },
            Reminder = new List<string> { "15m", "30m", "1h", "1d" },
            Location = new List<SimpleItemDto> { new() { Id = "virtual", Label = "Virtual" }, new() { Id = "onsite", Label = "Onsite" }, new() { Id = "in_person", Label = "In Person" } },
            SuggestedSlots = new List<SuggestedSlotDto>
            {
                new() { Id = Guid.NewGuid().ToString(), Label = "Morning Slot", Start = DateTime.UtcNow.Date.AddHours(9).ToString("o"), Timezone = "UTC", DurationMinutes = 30 },
                new() { Id = Guid.NewGuid().ToString(), Label = "Afternoon Slot", Start = DateTime.UtcNow.Date.AddHours(14).ToString("o"), Timezone = "UTC", DurationMinutes = 45 }
            },
            DefaultTimezone = "UTC",
            MeetingProviders = _meetingProviders.Select(p => p.ProviderKey).ToList(),
            DefaultProvider = _meetingProviders.FirstOrDefault()?.ProviderKey ?? "google_meet"
        };
        return Task.FromResult(resp);
    }

    public async Task<InterviewCalendarDto> ScheduleAsync(ScheduleInterviewRequest request)
    {
        var interview = new Interview
        {
            CandidateId = request.CandidateId,
            JobId = request.JobId,
            Stage = request.Stage,
            StartUtc = request.StartUtc,
            DurationMinutes = request.DurationMinutes,
            Timezone = request.Timezone,
            LocationType = request.LocationType.ToLower() switch { "virtual" => InterviewLocationType.Virtual, "onsite" => InterviewLocationType.Onsite, "in_person" => InterviewLocationType.InPerson, _ => InterviewLocationType.Virtual },
            MeetingProvider = request.MeetingProvider,
            MeetingLink = request.MeetingLink,
            LocationDetail = request.LocationDetail,
            Notes = request.Notes,
            Reminders = string.Join(',', request.Reminders),
            Flags = string.Join(',', request.Flags),
            InterviewerIds = string.Join(',', request.InterviewerIds),
            InterviewerNames = string.Empty // can populate later
        };

        if (!string.IsNullOrWhiteSpace(request.MeetingProvider))
        {
            var provider = _meetingProviders.FirstOrDefault(p => p.ProviderKey == request.MeetingProvider);
            if (provider != null)
            {
                var meetingResult = await provider.CreateMeetingAsync(new MeetingRequest(request.MeetingProvider, $"Interview {interview.Stage}", interview.StartUtc, interview.DurationMinutes, interview.Timezone, request.InterviewerIds));
                if (meetingResult.Success)
                {
                    interview.MeetingLink = meetingResult.MeetingLink;
                    interview.ExternalEventId = meetingResult.ExternalEventId ?? string.Empty;
                }
            }
        }

        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync();

        return new InterviewCalendarDto
        {
            InterviewId = interview.Id,
            PublicId = interview.PublicId,
            CandidateId = interview.CandidateId,
            CandidateName = "",
            JobId = interview.JobId,
            JobTitle = "",
            Stage = interview.Stage,
            InterviewerId = null,
            InterviewerName = interview.InterviewerNames,
            Start = interview.StartUtc.ToString("o"),
            DurationMinutes = interview.DurationMinutes,
            Timezone = interview.Timezone,
            Label = $"{interview.Stage} - {interview.StartUtc:yyyy-MM-dd HH:mm}",
            MeetingLink = interview.MeetingLink,
            Type = interview.LocationType switch { InterviewLocationType.Virtual => "virtual", InterviewLocationType.Onsite => "onsite", InterviewLocationType.InPerson => "in_person", _ => "virtual" }
        };
    }
}
