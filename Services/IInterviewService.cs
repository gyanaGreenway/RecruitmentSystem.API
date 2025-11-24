using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public interface IInterviewService
{
    Task<InterviewCalendarResponse> GetCalendarAsync(int? jobId, string? stage, int? interviewerId, DateTime? startDate, DateTime? endDate);
    Task<List<InterviewFeedbackDto>> GetFeedbackAsync(InterviewFeedbackFilter filter);
    Task<InterviewMetadataResponse> GetMetadataAsync();
    Task<InterviewCalendarDto> ScheduleAsync(ScheduleInterviewRequest request);
}
