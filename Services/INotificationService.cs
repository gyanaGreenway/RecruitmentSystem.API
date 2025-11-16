using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public interface INotificationService
{
 Task<List<NotificationDto>> GetNotificationsForCandidateAsync(int candidateId);
 Task<int> GetUnreadCountAsync(int candidateId);
 Task MarkAsReadAsync(int id);
 Task MarkAllReadAsync(int candidateId);
 Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createDto);
 Task<bool> DeleteNotificationAsync(int id);
}
