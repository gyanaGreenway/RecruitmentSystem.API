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

    private static CandidateDto Map(Candidate c)
    {
        return new CandidateDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Phone = c.Phone,
            ResumeUrl = c.ResumeUrl,
            ResumeHeadline = c.ResumeHeadline,
            KeySkills = (c.KeySkills ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            ProfileSummary = c.ProfileSummary,
            Accomplishments = c.Accomplishments,
            CareerProfile = c.CareerProfile,
            PersonalDetails = new CandidatePersonalDetailsDto
            {
                DateOfBirth = c.DateOfBirth,
                Gender = c.Gender,
                Nationality = c.Nationality,
                MaritalStatus = c.MaritalStatus,
                Address = c.Address,
                City = c.City,
                State = c.State,
                ZipCode = c.ZipCode,
                Country = c.Country
            },
            Employment = c.Employment.Select(e => new CandidateEmploymentItemDto
            {
                Id = e.Id,
                JobTitle = e.JobTitle,
                Company = e.Company,
                WorkArea = e.WorkArea,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                CurrentlyWorking = e.CurrentlyWorking,
                Description = e.Description
            }).ToList(),
            Education = c.Education.Select(e => new CandidateEducationItemDto
            {
                Id = e.Id,
                Degree = e.Degree,
                Field = e.Field,
                Institution = e.Institution,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Grade = e.Grade,
                Description = e.Description
            }).ToList(),
            ITSkills = c.ITSkills.Select(s => new CandidateSkillItemDto
            {
                Id = s.Id,
                Skill = s.Skill,
                Proficiency = s.Proficiency
            }).ToList(),
            Projects = c.Projects.Select(p => new CandidateProjectItemDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Link = p.Link,
                StartDate = p.StartDate,
                EndDate = p.EndDate
            }).ToList(),
            IsDeleted = c.IsDeleted,
            RowVersion = c.RowVersion != null ? Convert.ToBase64String(c.RowVersion) : null
        };
    }

    public async Task<PagedResultDto<CandidateDto>> GetAllCandidatesAsync(int pageNumber, int pageSize)
    {
        var query = _context.Candidates
            .Include(c => c.Employment)
            .Include(c => c.Education)
            .Include(c => c.ITSkills)
            .Include(c => c.Projects)
            .AsQueryable();
        var totalCount = await query.CountAsync();
        var candidates = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => Map(c))
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
        var candidate = await _context.Candidates
            .Include(c => c.Employment)
            .Include(c => c.Education)
            .Include(c => c.ITSkills)
            .Include(c => c.Projects)
            .FirstOrDefaultAsync(c => c.Id == id);
        return candidate == null ? null : Map(candidate);
    }

    public async Task<CandidateDto> CreateCandidateAsync(CreateCandidateDto dto)
    {
        var candidate = new Candidate
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            ResumeUrl = dto.ResumeUrl ?? string.Empty,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 11),
            ResumeHeadline = dto.ResumeHeadline,
            KeySkills = string.Join(',', dto.KeySkills ?? new List<string>()),
            ProfileSummary = dto.ProfileSummary,
            Accomplishments = dto.Accomplishments,
            CareerProfile = dto.CareerProfile,
            DateOfBirth = dto.PersonalDetails?.DateOfBirth,
            Gender = dto.PersonalDetails?.Gender ?? string.Empty,
            Nationality = dto.PersonalDetails?.Nationality ?? string.Empty,
            MaritalStatus = dto.PersonalDetails?.MaritalStatus ?? string.Empty,
            Address = dto.PersonalDetails?.Address ?? string.Empty,
            City = dto.PersonalDetails?.City ?? string.Empty,
            State = dto.PersonalDetails?.State ?? string.Empty,
            ZipCode = dto.PersonalDetails?.ZipCode ?? string.Empty,
            Country = dto.PersonalDetails?.Country ?? string.Empty
        };
        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();

        // User record
        _context.Users.Add(new User { Email = candidate.Email, PasswordHash = candidate.PasswordHash, Role = UserRole.Candidate });
        await _context.SaveChangesAsync();

        // Child collections (after candidate Id exists)
        if (dto.Employment.Any())
        {
            foreach (var e in dto.Employment)
            {
                _context.CandidateEmployment.Add(new CandidateEmployment
                {
                    CandidateId = candidate.Id,
                    JobTitle = e.JobTitle,
                    Company = e.Company,
                    WorkArea = e.WorkArea,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    CurrentlyWorking = e.CurrentlyWorking,
                    Description = e.Description
                });
            }
        }
        if (dto.Education.Any())
        {
            foreach (var e in dto.Education)
            {
                _context.CandidateEducation.Add(new CandidateEducation
                {
                    CandidateId = candidate.Id,
                    Degree = e.Degree,
                    Field = e.Field,
                    Institution = e.Institution,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Grade = e.Grade,
                    Description = e.Description
                });
            }
        }
        if (dto.ITSkills.Any())
        {
            foreach (var s in dto.ITSkills)
            {
                _context.CandidateSkills.Add(new CandidateSkill
                {
                    CandidateId = candidate.Id,
                    Skill = s.Skill,
                    Proficiency = s.Proficiency
                });
            }
        }
        if (dto.Projects.Any())
        {
            foreach (var p in dto.Projects)
            {
                _context.CandidateProjects.Add(new CandidateProject
                {
                    CandidateId = candidate.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Link = p.Link,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                });
            }
        }
        if (dto.Employment.Any() || dto.Education.Any() || dto.ITSkills.Any() || dto.Projects.Any())
        {
            await _context.SaveChangesAsync();
        }

        return await GetCandidateByIdAsync(candidate.Id) ?? Map(candidate);
    }

    public async Task<CandidateDto?> UpdateCandidateAsync(int id, UpdateCandidateDto dto)
    {
        var candidate = await _context.Candidates
            .Include(c => c.Employment)
            .Include(c => c.Education)
            .Include(c => c.ITSkills)
            .Include(c => c.Projects)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (candidate == null) return null;

        // Concurrency check
        var currentVersion = candidate.RowVersion != null ? Convert.ToBase64String(candidate.RowVersion) : null;
        if (currentVersion != dto.RowVersion) throw new InvalidOperationException("Candidate record has been modified by another process.");

        candidate.FirstName = dto.FirstName;
        candidate.LastName = dto.LastName;
        candidate.Email = dto.Email;
        candidate.Phone = dto.Phone;
        candidate.ResumeUrl = dto.ResumeUrl ?? string.Empty;
        candidate.ResumeHeadline = dto.ResumeHeadline;
        candidate.KeySkills = string.Join(',', dto.KeySkills ?? new List<string>());
        candidate.ProfileSummary = dto.ProfileSummary;
        candidate.Accomplishments = dto.Accomplishments;
        candidate.CareerProfile = dto.CareerProfile;
        candidate.DateOfBirth = dto.PersonalDetails?.DateOfBirth;
        candidate.Gender = dto.PersonalDetails?.Gender ?? string.Empty;
        candidate.Nationality = dto.PersonalDetails?.Nationality ?? string.Empty;
        candidate.MaritalStatus = dto.PersonalDetails?.MaritalStatus ?? string.Empty;
        candidate.Address = dto.PersonalDetails?.Address ?? string.Empty;
        candidate.City = dto.PersonalDetails?.City ?? string.Empty;
        candidate.State = dto.PersonalDetails?.State ?? string.Empty;
        candidate.ZipCode = dto.PersonalDetails?.ZipCode ?? string.Empty;
        candidate.Country = dto.PersonalDetails?.Country ?? string.Empty;
        candidate.UpdatedAt = DateTime.UtcNow;

        // Replace child collections: simplistic approach (could optimize diffing)
        _context.CandidateEmployment.RemoveRange(candidate.Employment);
        _context.CandidateEducation.RemoveRange(candidate.Education);
        _context.CandidateSkills.RemoveRange(candidate.ITSkills);
        _context.CandidateProjects.RemoveRange(candidate.Projects);
        await _context.SaveChangesAsync();

        foreach (var e in dto.Employment)
        {
            _context.CandidateEmployment.Add(new CandidateEmployment
            {
                CandidateId = candidate.Id,
                JobTitle = e.JobTitle,
                Company = e.Company,
                WorkArea = e.WorkArea,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                CurrentlyWorking = e.CurrentlyWorking,
                Description = e.Description
            });
        }
        foreach (var e in dto.Education)
        {
            _context.CandidateEducation.Add(new CandidateEducation
            {
                CandidateId = candidate.Id,
                Degree = e.Degree,
                Field = e.Field,
                Institution = e.Institution,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Grade = e.Grade,
                Description = e.Description
            });
        }
        foreach (var s in dto.ITSkills)
        {
            _context.CandidateSkills.Add(new CandidateSkill
            {
                CandidateId = candidate.Id,
                Skill = s.Skill,
                Proficiency = s.Proficiency
            });
        }
        foreach (var p in dto.Projects)
        {
            _context.CandidateProjects.Add(new CandidateProject
            {
                CandidateId = candidate.Id,
                Title = p.Title,
                Description = p.Description,
                Link = p.Link,
                StartDate = p.StartDate,
                EndDate = p.EndDate
            });
        }
        await _context.SaveChangesAsync();

        return await GetCandidateByIdAsync(candidate.Id);
    }

    public async Task<bool> DeleteCandidateAsync(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null) return false;
        candidate.IsDeleted = true; // soft delete
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<CandidateDto>> SearchCandidatesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<CandidateDto>();
        query = query.ToLower().Trim();
        return await _context.Candidates
            .Include(c => c.Employment)
            .Include(c => c.Education)
            .Include(c => c.ITSkills)
            .Include(c => c.Projects)
            .Where(c => c.FirstName.ToLower().Contains(query) || c.LastName.ToLower().Contains(query) || c.Email.ToLower().Contains(query) || c.KeySkills.ToLower().Contains(query))
            .Select(c => Map(c))
            .ToListAsync();
    }

    public async Task<int> StartCandidateRegistrationAsync(StartCandidateRegistrationDto dto)
    {
        var existsUser = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        var existsCandidate = await _context.Candidates.AnyAsync(c => c.Email == dto.Email);
        if (existsUser || existsCandidate) throw new InvalidOperationException("An account with this email already exists.");
        var otp = GenerateNumericOtp(6);
        var rec = new CandidateRegistrationRequest
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            ResumeUrl = dto.ResumeUrl ?? string.Empty,
            KeySkills = dto.KeySkillsRaw,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            OtpHash = BCrypt.Net.BCrypt.HashPassword(otp),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            Attempts = 0,
            Verified = false
        };
        _context.CandidateRegistrationRequests.Add(rec);
        await _context.SaveChangesAsync();
        return rec.Id;
    }

    public async Task<CandidateDto> ConfirmCandidateRegistrationAsync(ConfirmCandidateRegistrationDto dto)
    {
        var rec = await _context.CandidateRegistrationRequests.FirstOrDefaultAsync(r => r.Id == dto.RequestId);
        if (rec == null || rec.ExpiresAt < DateTime.UtcNow) throw new InvalidOperationException("Registration request expired or not found.");
        if (rec.Verified)
        {
            var existing = await _context.Candidates
                .Include(c => c.Employment)
                .Include(c => c.Education)
                .Include(c => c.ITSkills)
                .Include(c => c.Projects)
                .FirstOrDefaultAsync(c => c.Email == rec.Email);
            if (existing != null) return Map(existing);
        }
        rec.Attempts++;
        if (!BCrypt.Net.BCrypt.Verify(dto.Otp, rec.OtpHash)) { await _context.SaveChangesAsync(); throw new InvalidOperationException("Invalid verification code."); }
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
        _context.Users.Add(new User { Email = rec.Email, PasswordHash = rec.PasswordHash, Role = UserRole.Candidate });
        await _context.SaveChangesAsync();
        return Map(candidate);
    }

    private static string GenerateNumericOtp(int length)
    {
        var rnd = new Random();
        return string.Concat(Enumerable.Range(0, length).Select(_ => rnd.Next(0, 10).ToString()));
    }
}

