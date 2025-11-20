using Microsoft.EntityFrameworkCore;
using RecruitmentSystem.API.Models;

namespace RecruitmentSystem.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
    public DbSet<CandidateRegistrationRequest> CandidateRegistrationRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Prevent duplicate applications (same Job + Candidate + Cycle)
        modelBuilder.Entity<Application>()
            .HasIndex(a => new { a.JobId, a.CandidateId, a.Cycle })
            .IsUnique();

        // Prevent duplicate job titles
        modelBuilder.Entity<Job>()
            .HasIndex(j => j.Title)
            .IsUnique();

        // Configure PublicId defaults and uniqueness
        modelBuilder.Entity<Job>()
            .Property(j => j.PublicId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");
        modelBuilder.Entity<Job>()
            .HasIndex(j => j.PublicId)
            .IsUnique();

<<<<<<< HEAD
        // Defaults for cycles
        modelBuilder.Entity<Job>()
            .Property(j => j.Cycle)
            .HasDefaultValue(1);
        modelBuilder.Entity<Application>()
            .Property(a => a.Cycle)
            .HasDefaultValue(1);
=======
        modelBuilder.Entity<PasswordResetRequest>()
            .HasIndex(r => new { r.UserId, r.Verified, r.ExpiresAt });

        modelBuilder.Entity<CandidateRegistrationRequest>()
            .HasIndex(r => new { r.Email, r.Verified, r.ExpiresAt });
>>>>>>> 0dde3112cc163ba687254a43f11c790e5fb680d7

        // Configure relationships
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

        // Indexes for performance
        modelBuilder.Entity<Candidate>()
            .HasIndex(c => c.Email);

        modelBuilder.Entity<Job>()
            .HasIndex(j => j.IsActive);

        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.CandidateId, n.Read });
    }
}

