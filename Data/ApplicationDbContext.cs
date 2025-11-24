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
    public DbSet<OfferLetter> OfferLetters { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<InterviewFeedback> InterviewFeedbacks { get; set; }
    public DbSet<Onboarding> Onboardings { get; set; }
    public DbSet<OnboardingTask> OnboardingTasks { get; set; }
    public DbSet<OnboardingDocument> OnboardingDocuments { get; set; }

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

        modelBuilder.Entity<OfferLetter>()
            .HasIndex(o => new { o.CandidateId, o.JobId });
        modelBuilder.Entity<OfferLetter>()
            .Property(o => o.Status)
            .HasConversion<int>();
        modelBuilder.Entity<OfferLetter>()
            .Property(o => o.AcceptanceProbability)
            .HasPrecision(5,2); // store as decimal if changed later
        modelBuilder.Entity<OfferLetter>()
            .Property(o => o.PublicId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");
        modelBuilder.Entity<OfferLetter>()
            .HasIndex(o => o.PublicId)
            .IsUnique();

        modelBuilder.Entity<Interview>()
            .Property(i => i.PublicId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");
        modelBuilder.Entity<Interview>()
            .HasIndex(i => i.PublicId)
            .IsUnique();
        modelBuilder.Entity<Interview>()
            .HasIndex(i => new { i.CandidateId, i.JobId, i.Stage });

        modelBuilder.Entity<InterviewFeedback>()
            .Property(f => f.PublicId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");
        modelBuilder.Entity<InterviewFeedback>()
            .HasIndex(f => f.PublicId)
            .IsUnique();
        modelBuilder.Entity<InterviewFeedback>()
            .HasIndex(f => new { f.InterviewId, f.InterviewerId });

        modelBuilder.Entity<Onboarding>()
            .Property(o => o.PublicId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");
        modelBuilder.Entity<Onboarding>()
            .HasIndex(o => o.PublicId)
            .IsUnique();
        modelBuilder.Entity<Onboarding>()
            .HasIndex(o => new { o.CandidateId, o.JobId });
        modelBuilder.Entity<Onboarding>()
            .HasMany(o => o.Tasks)
            .WithOne()
            .HasForeignKey(t => t.OnboardingId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Onboarding>()
            .HasMany(o => o.Documents)
            .WithOne()
            .HasForeignKey(d => d.OnboardingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OnboardingTask>()
            .Property(t => t.PublicId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");
        modelBuilder.Entity<OnboardingTask>()
            .HasIndex(t => t.PublicId)
            .IsUnique();

        modelBuilder.Entity<OnboardingDocument>()
            .Property(d => d.PublicId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");
        modelBuilder.Entity<OnboardingDocument>()
            .HasIndex(d => d.PublicId)
            .IsUnique();

        modelBuilder.Entity<Job>()
            .HasIndex(j => j.IsActive);
        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.CandidateId, n.Read });
    }
}

