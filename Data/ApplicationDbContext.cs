using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
    public DbSet<CandidateRegistrationRequest> CandidateRegistrationRequests { get; set; }
    public DbSet<CandidateEmployment> CandidateEmployment { get; set; }
    public DbSet<CandidateEducation> CandidateEducation { get; set; }
    public DbSet<CandidateSkill> CandidateSkills { get; set; }
    public DbSet<CandidateProject> CandidateProjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Application>()
            .HasIndex(a => new { a.JobId, a.CandidateId, a.Cycle })
            .IsUnique();
        modelBuilder.Entity<Job>()
            .HasIndex(j => j.Title)
            .IsUnique();
        modelBuilder.Entity<Job>()
            .Property(j => j.PublicId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");
        modelBuilder.Entity<Job>()
            .HasIndex(j => j.PublicId)
            .IsUnique();
        modelBuilder.Entity<Job>()
            .Property(j => j.Cycle)
            .HasDefaultValue(1);
        modelBuilder.Entity<Application>()
            .Property(a => a.Cycle)
            .HasDefaultValue(1);
        modelBuilder.Entity<Job>()
            .Property(j => j.Salary)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Candidate>()
            .Property(c => c.RowVersion)
            .IsRowVersion();
        modelBuilder.Entity<Candidate>()
            .HasIndex(c => c.Email);
        // Removed global query filter to prevent required end warnings; implement soft delete at service layer.

        modelBuilder.Entity<PasswordResetRequest>()
            .HasIndex(r => new { r.UserId, r.Verified, r.ExpiresAt });
        modelBuilder.Entity<CandidateRegistrationRequest>()
            .HasIndex(r => new { r.Email, r.Verified, r.ExpiresAt });

        modelBuilder.Entity<Application>()
            .HasOne(a => a.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Application>()
            .HasOne(a => a.Candidate)
            .WithMany(c => c.Applications)
            .HasForeignKey(a => a.CandidateId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ApplicationStatusHistory>()
            .HasOne(h => h.Application)
            .WithMany(a => a.StatusHistory)
            .HasForeignKey(h => h.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CandidateEmployment>()
            .HasOne(e => e.Candidate)
            .WithMany(c => c.Employment)
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CandidateEducation>()
            .HasOne(e => e.Candidate)
            .WithMany(c => c.Education)
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CandidateSkill>()
            .HasOne(s => s.Candidate)
            .WithMany(c => c.ITSkills)
            .HasForeignKey(s => s.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CandidateProject>()
            .HasOne(p => p.Candidate)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Job>()
            .HasIndex(j => j.IsActive);
        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.CandidateId, n.Read });
    }
}

