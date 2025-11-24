namespace RecruitmentSystem.API.Services;

public record MeetingRequest(string Provider, string Topic, DateTime StartUtc, int DurationMinutes, string Timezone, List<int> InterviewerIds);
public record MeetingResult(bool Success, string? MeetingLink, string? ExternalEventId, string? Error);

public interface IMeetingProvider
{
    string ProviderKey { get; }
    Task<MeetingResult> CreateMeetingAsync(MeetingRequest request);
}

public class GoogleMeetProvider : IMeetingProvider
{
    public string ProviderKey => "google_meet";
    public Task<MeetingResult> CreateMeetingAsync(MeetingRequest request)
    {
        // Placeholder: integrate Google Calendar API
        return Task.FromResult(new MeetingResult(true, "https://meet.google.com/dummy", Guid.NewGuid().ToString(), null));
    }
}

public class TeamsProvider : IMeetingProvider
{
    public string ProviderKey => "microsoft_teams";
    public Task<MeetingResult> CreateMeetingAsync(MeetingRequest request)
    {
        // Placeholder: integrate Microsoft Graph onlineMeetings
        return Task.FromResult(new MeetingResult(true, "https://teams.microsoft.com/l/meetup-join/dummy", Guid.NewGuid().ToString(), null));
    }
}

public class ZoomProvider : IMeetingProvider
{
    public string ProviderKey => "zoom";
    public Task<MeetingResult> CreateMeetingAsync(MeetingRequest request)
    {
        // Placeholder: integrate Zoom REST API
        return Task.FromResult(new MeetingResult(true, "https://zoom.us/j/123456789", Guid.NewGuid().ToString(), null));
    }
}
