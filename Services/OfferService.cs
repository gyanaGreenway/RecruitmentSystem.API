using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.Services;

public class OfferService : IOfferService
{
    private readonly ApplicationDbContext _context;

    public OfferService(ApplicationDbContext context)
    {
        _context = context;
    }

    private static OfferLetterDto Map(OfferLetter o) => new()
    {
        Id = o.Id,
        PublicId = o.PublicId,
        Status = o.Status.ToString(),
        CandidateId = o.CandidateId,
        CandidateName = o.CandidateName,
        CandidateEmail = o.CandidateEmail,
        JobId = o.JobId,
        JobTitle = o.JobTitle,
        RecruiterId = o.RecruiterId,
        RecruiterName = o.RecruiterName,
        SentOn = o.SentOn,
        TargetStart = o.TargetStart,
        LastTouched = o.LastTouched,
        Compensation = o.Compensation,
        Location = o.Location,
        Attachments = o.AttachmentsCount,
        Notes = o.Notes,
        AcceptanceProbability = o.AcceptanceProbability,
        OfferLink = o.OfferLink
    };

    public async Task<PagedResultOfferDto> GetOffersAsync(OfferFilterDto filter)
    {
        var query = _context.OfferLetters.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<OfferStatus>(filter.Status, true, out var status))
        {
            query = query.Where(o => o.Status == status);
        }
        if (filter.RecruiterId.HasValue)
        {
            query = query.Where(o => o.RecruiterId == filter.RecruiterId.Value);
        }
        if (filter.CandidateId.HasValue)
        {
            query = query.Where(o => o.CandidateId == filter.CandidateId.Value);
        }
        if (filter.JobId.HasValue)
        {
            query = query.Where(o => o.JobId == filter.JobId.Value);
        }
        if (filter.StartDate.HasValue)
        {
            query = query.Where(o => o.SentOn >= filter.StartDate);
        }
        if (filter.EndDate.HasValue)
        {
            query = query.Where(o => o.SentOn <= filter.EndDate);
        }
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(o => o.CandidateName.ToLower().Contains(term) || o.JobTitle.ToLower().Contains(term) || o.Compensation.ToLower().Contains(term));
        }

        query = filter.SortBy?.ToLower() switch
        {
            "status" => filter.SortOrder?.ToLower() == "asc" ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
            "candidate" => filter.SortOrder?.ToLower() == "asc" ? query.OrderBy(o => o.CandidateName) : query.OrderByDescending(o => o.CandidateName),
            "senton" => filter.SortOrder?.ToLower() == "asc" ? query.OrderBy(o => o.SentOn) : query.OrderByDescending(o => o.SentOn),
            "lasttouched" => filter.SortOrder?.ToLower() == "asc" ? query.OrderBy(o => o.LastTouched) : query.OrderByDescending(o => o.LastTouched),
            _ => query.OrderByDescending(o => o.SentOn)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(o => Map(o))
            .ToListAsync();

        return new PagedResultOfferDto
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<OfferLetterDto?> GetOfferAsync(int id)
    {
        var offer = await _context.OfferLetters.FindAsync(id);
        return offer == null ? null : Map(offer);
    }

    public async Task<OfferLetterDto> CreateOfferAsync(CreateOfferLetterDto dto)
    {
        // For now populate names/emails from related tables (best effort)
        var candidate = await _context.Candidates.FindAsync(dto.CandidateId);
        var job = await _context.Jobs.FindAsync(dto.JobId);
        var recruiter = dto.RecruiterId.HasValue ? await _context.Users.FindAsync(dto.RecruiterId.Value) : null;

        var offer = new OfferLetter
        {
            CandidateId = dto.CandidateId,
            JobId = dto.JobId,
            RecruiterId = dto.RecruiterId,
            CandidateName = candidate != null ? candidate.FirstName + " " + candidate.LastName : string.Empty,
            CandidateEmail = candidate?.Email ?? string.Empty,
            JobTitle = job?.Title ?? string.Empty,
            RecruiterName = recruiter?.Email ?? string.Empty,
            SentOn = DateTime.UtcNow,
            TargetStart = dto.TargetStart,
            LastTouched = DateTime.UtcNow,
            Compensation = dto.Compensation,
            Location = dto.Location,
            AttachmentsCount = dto.Attachments,
            Notes = dto.Notes,
            AcceptanceProbability = dto.AcceptanceProbability,
            OfferLink = dto.OfferLink,
            Status = OfferStatus.Draft
        };
        _context.OfferLetters.Add(offer);
        await _context.SaveChangesAsync();
        return Map(offer);
    }

    public async Task<OfferLetterDto?> UpdateOfferAsync(int id, UpdateOfferLetterDto dto)
    {
        var offer = await _context.OfferLetters.FindAsync(id);
        if (offer == null) return null;
        if (Enum.TryParse<OfferStatus>(dto.Status, true, out var status))
        {
            offer.Status = status;
        }
        offer.Compensation = dto.Compensation;
        offer.Location = dto.Location;
        offer.AttachmentsCount = dto.Attachments;
        offer.Notes = dto.Notes;
        offer.AcceptanceProbability = dto.AcceptanceProbability;
        offer.TargetStart = dto.TargetStart;
        offer.OfferLink = dto.OfferLink;
        offer.LastTouched = DateTime.UtcNow;
        offer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Map(offer);
    }

    public async Task<bool> DeleteOfferAsync(int id)
    {
        var offer = await _context.OfferLetters.FindAsync(id);
        if (offer == null) return false;
        _context.OfferLetters.Remove(offer);
        await _context.SaveChangesAsync();
        return true;
    }
}
