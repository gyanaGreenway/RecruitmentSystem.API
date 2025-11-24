using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.Services;

public class BackgroundVerificationService : IBackgroundVerificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IEncryptionService _encryption;

    public BackgroundVerificationService(ApplicationDbContext context, IEncryptionService encryption)
    {
        _context = context;
        _encryption = encryption;
    }

    private BackgroundVerificationDto Map(BackgroundVerification b) => new()
    {
        Id = b.Id,
        PublicId = b.PublicId,
        CandidateId = b.CandidateId,
        JobId = b.JobId,
        Provider = b.Provider,
        Stages = b.Stages?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>(),
        Status = b.Status.ToString(),
        Risk = b.Risk.ToString(),
        ChecksRun = b.ChecksRun,
        IssuesFound = b.IssuesFound,
        StartedAt = b.StartedAt?.ToString("o"),
        CompletedAt = b.CompletedAt?.ToString("o"),
        TurnaroundDays = b.TurnaroundDays,
        Notes = b.Notes,
        ExternalId = b.ExternalId,
        CreatedAt = b.CreatedAt,
        UpdatedAt = b.UpdatedAt,
        Documents = b.Documents.Select(d => new BackgroundVerificationDocumentDto
        {
            Id = d.Id,
            PublicId = d.PublicId,
            FileName = d.FileName,
            Url = string.IsNullOrEmpty(d.Url) ? string.Empty : _encryption.Unprotect(d.Url),
            FileType = d.FileType,
            FileSize = d.FileSize,
            Comments = d.Comments,
            UploadedById = d.UploadedById,
            UploadedAt = d.UploadedAt
        }).ToList()
    };

    public async Task<PagedResultDto<BackgroundVerificationDto>> GetAllAsync(int pageNumber = 1, int pageSize = 25, string? status = null)
    {
        var query = _context.BackgroundVerifications.Include(b => b.Documents).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BgStatus>(status, true, out var st)) query = query.Where(b => b.Status == st);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(b => b.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResultDto<BackgroundVerificationDto> { Items = items.Select(Map).ToList(), TotalCount = total, PageNumber = pageNumber, PageSize = pageSize };
    }

    public async Task<BackgroundVerificationDto?> GetByIdAsync(int id)
    {
        var b = await _context.BackgroundVerifications.Include(x => x.Documents).FirstOrDefaultAsync(x => x.Id == id);
        return b == null ? null : Map(b);
    }

    public async Task<BackgroundVerificationDto> CreateAsync(CreateBackgroundVerificationDto dto, int createdById)
    {
        var b = new BackgroundVerification
        {
            CandidateId = dto.CandidateId,
            JobId = dto.JobId,
            Provider = dto.Provider,
            Stages = dto.Stages != null ? string.Join(',', dto.Stages) : string.Empty,
            Status = BgStatus.Pending,
            Risk = BgRisk.Low,
            ChecksRun = 0,
            IssuesFound = 0,
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        _context.BackgroundVerifications.Add(b);
        await _context.SaveChangesAsync();

        foreach (var d in dto.Documents)
        {
            _context.BackgroundVerificationDocuments.Add(new BackgroundVerificationDocument
            {
                BackgroundVerificationId = b.Id,
                FileName = d.FileName,
                Url = string.IsNullOrEmpty(d.Url) ? string.Empty : _encryption.Protect(d.Url),
                FileType = d.FileType,
                FileSize = d.FileSize,
                Comments = d.Comments ?? string.Empty,
                UploadedById = createdById,
                UploadedAt = DateTime.UtcNow
            });
        }
        await _context.SaveChangesAsync();
        return await GetByIdAsync(b.Id) ?? Map(b);
    }

    public async Task<bool> CancelAsync(int id)
    {
        var b = await _context.BackgroundVerifications.FindAsync(id);
        if (b == null) return false;
        b.Status = BgStatus.Exceptions;
        b.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BackgroundVerificationDto?> UpdateStatusAsync(int id, string status, string? notes = null)
    {
        var b = await _context.BackgroundVerifications.Include(x => x.Documents).FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return null;
        if (Enum.TryParse<BgStatus>(status, true, out var st)) b.Status = st;
        if (!string.IsNullOrWhiteSpace(notes)) b.Notes = notes;
        if (b.Status == BgStatus.Cleared || b.Status == BgStatus.Review) b.CompletedAt = DateTime.UtcNow;
        b.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Map(b);
    }
}
