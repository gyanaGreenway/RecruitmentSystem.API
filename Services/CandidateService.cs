using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;

namespace RecruitmentSystem.API.Services;

public class CandidateService : ICandidateService
{
    private readonly ApplicationDbContext _context;

    public CandidateService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<CandidateDto>> GetAllCandidatesAsync(int pageNumber, int pageSize)
    {
        var query = _context.Candidates.AsQueryable();
        var totalCount = await query.CountAsync();

        var candidates = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CandidateDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                ResumeUrl = c.ResumeUrl,
                KeySkills = c.KeySkills
            })
            .ToListAsync();

        return new PagedResultDto<CandidateDto>
        {
            Items = candidates,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<CandidateDto?> GetCandidateByIdAsync(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null) return null;

        return new CandidateDto
        {
            Id = candidate.Id,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Email = candidate.Email,
            Phone = candidate.Phone,
            ResumeUrl = candidate.ResumeUrl,
            KeySkills = candidate.KeySkills
        };
    }

    public async Task<CandidateDto> CreateCandidateAsync(CreateCandidateDto createCandidateDto)
    {
        var candidate = new Models.Candidate
        {
            FirstName = createCandidateDto.FirstName,
            LastName = createCandidateDto.LastName,
            Email = createCandidateDto.Email,
            Phone = createCandidateDto.Phone,
            ResumeUrl = createCandidateDto.ResumeUrl,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createCandidateDto.Password),
            KeySkills = createCandidateDto.KeySkills
        };

        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();

        // Also create a User record for login
        var user = new Models.User
        {
            Email = createCandidateDto.Email,
            PasswordHash = candidate.PasswordHash,
            Role = Models.UserRole.Candidate
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new CandidateDto
        {
            Id = candidate.Id,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Email = candidate.Email,
            Phone = candidate.Phone,
            ResumeUrl = candidate.ResumeUrl,
            KeySkills = candidate.KeySkills
        };
    }

    public async Task<CandidateDto?> UpdateCandidateAsync(int id, UpdateCandidateDto updateCandidateDto)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null) return null;

        candidate.FirstName = updateCandidateDto.FirstName;
        candidate.LastName = updateCandidateDto.LastName;
        candidate.Email = updateCandidateDto.Email;
        candidate.Phone = updateCandidateDto.Phone;
        candidate.ResumeUrl = updateCandidateDto.ResumeUrl;
        candidate.KeySkills = updateCandidateDto.KeySkills;
        candidate.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new CandidateDto
        {
            Id = candidate.Id,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Email = candidate.Email,
            Phone = candidate.Phone,
            ResumeUrl = candidate.ResumeUrl,
            KeySkills = candidate.KeySkills
        };
    }

    public async Task<bool> DeleteCandidateAsync(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null) return false;

        _context.Candidates.Remove(candidate);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<CandidateDto>> SearchCandidatesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<CandidateDto>();

        query = query.ToLower().Trim();

        var results = await _context.Candidates
            .Where(c => c.FirstName.ToLower().Contains(query) || c.LastName.ToLower().Contains(query) || c.Email.ToLower().Contains(query) || c.KeySkills.ToLower().Contains(query))
            .Select(c => new CandidateDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                ResumeUrl = c.ResumeUrl,
                KeySkills = c.KeySkills
            })
            .ToListAsync();

        return results;
    }
}

