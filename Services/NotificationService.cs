using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationDto>> GetNotificationsForCandidateAsync(int candidateId)
    {
        return await _context.Notifications
        .Where(n => n.CandidateId == candidateId)
        .OrderByDescending(n => n.CreatedAt)
        .Select(n => new NotificationDto
        {
            Id = n.Id,
            CandidateId = n.CandidateId,
            JobId = n.JobId,
            JobTitle = n.JobTitle,
            Message = n.Message,
            MatchPercentage = n.MatchPercentage,
            Type = n.Type,
            Read = n.Read,
            CreatedAt = n.CreatedAt,
            ActionUrl = n.ActionUrl
        })
        .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int candidateId)
    {
        return await _context.Notifications.CountAsync(n => n.CandidateId == candidateId && !n.Read);
    }

    public async Task MarkAsReadAsync(int id)
    {
        var n = await _context.Notifications.FindAsync(id);
        if (n == null) return;
        n.Read = true;
        await _context.SaveChangesAsync();
    }

    public async Task MarkAllReadAsync(int candidateId)
    {
        var notifications = await _context.Notifications.Where(n => n.CandidateId == candidateId && !n.Read).ToListAsync();
        foreach (var n in notifications) n.Read = true;
        await _context.SaveChangesAsync();
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createDto)
    {
        var n = new Notification
        {
            CandidateId = createDto.CandidateId,
            JobId = createDto.JobId,
            JobTitle = createDto.JobTitle,
            Message = createDto.Message,
            MatchPercentage = createDto.MatchPercentage,
            Type = createDto.Type,
            ActionUrl = createDto.ActionUrl
        };
        _context.Notifications.Add(n);
        await _context.SaveChangesAsync();

        return new NotificationDto
        {
            Id = n.Id,
            CandidateId = n.CandidateId,
            JobId = n.JobId,
            JobTitle = n.JobTitle,
            Message = n.Message,
            MatchPercentage = n.MatchPercentage,
            Type = n.Type,
            Read = n.Read,
            CreatedAt = n.CreatedAt,
            ActionUrl = n.ActionUrl
        };
    }

    public async Task<bool> DeleteNotificationAsync(int id)
    {
        var n = await _context.Notifications.FindAsync(id);
        if (n == null) return false;
        _context.Notifications.Remove(n);
        await _context.SaveChangesAsync();
        return true;
    }
}
