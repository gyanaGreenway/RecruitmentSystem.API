using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Models;

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

    public async Task<int> StartCandidateRegistrationAsync(StartCandidateRegistrationDto dto)
    {
        // Reject if email already exists as user or candidate
        var existsUser = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        var existsCandidate = await _context.Candidates.AnyAsync(c => c.Email == dto.Email);
        if (existsUser || existsCandidate)
        {
            throw new InvalidOperationException("An account with this email already exists.");
        }

        // Generate OTP and store hashed request
        var otp = GenerateNumericOtp(6);
        var rec = new CandidateRegistrationRequest
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            ResumeUrl = dto.ResumeUrl,
            KeySkills = dto.KeySkills,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            OtpHash = BCrypt.Net.BCrypt.HashPassword(otp),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            Attempts = 0,
            Verified = false
        };
        _context.CandidateRegistrationRequests.Add(rec);
        await _context.SaveChangesAsync();

        // TODO: send OTP via email provider
        return rec.Id;
    }

    public async Task<CandidateDto> ConfirmCandidateRegistrationAsync(ConfirmCandidateRegistrationDto dto)
    {
        var rec = await _context.CandidateRegistrationRequests.FirstOrDefaultAsync(r => r.Id == dto.RequestId);
        if (rec == null || rec.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Registration request expired or not found.");
        }
        if (rec.Verified)
        {
            // Already created? try to find candidate
            var existing = await _context.Candidates.FirstOrDefaultAsync(c => c.Email == rec.Email);
            if (existing != null)
            {
                return new CandidateDto
                {
                    Id = existing.Id,
                    FirstName = existing.FirstName,
                    LastName = existing.LastName,
                    Email = existing.Email,
                    Phone = existing.Phone,
                    ResumeUrl = existing.ResumeUrl,
                    KeySkills = existing.KeySkills
                };
            }
        }

        rec.Attempts++;
        if (!BCrypt.Net.BCrypt.Verify(dto.Otp, rec.OtpHash))
        {
            await _context.SaveChangesAsync();
            throw new InvalidOperationException("Invalid verification code.");
        }

        // Mark verified and create account
        rec.Verified = true;
        await _context.SaveChangesAsync();

        var candidate = new Candidate
        {
            FirstName = rec.FirstName,
            LastName = rec.LastName,
            Email = rec.Email,
            Phone = rec.Phone,
            ResumeUrl = rec.ResumeUrl,
            PasswordHash = rec.PasswordHash,
            KeySkills = rec.KeySkills
        };
        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();

        var user = new User
        {
            Email = rec.Email,
            PasswordHash = rec.PasswordHash,
            Role = UserRole.Candidate
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // TODO: send confirmation email: "Your account was created successfully."

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

    private static string GenerateNumericOtp(int length)
    {
        var rnd = new Random();
        return string.Concat(Enumerable.Range(0, length).Select(_ => rnd.Next(0, 10).ToString()));
    }
}

