using Microsoft.EntityFrameworkCore;
using Nafes.API.Modules;

namespace Nafes.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameQuestion> GameQuestions { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<StudentAchievement> StudentAchievements { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<WheelGameResult> WheelGameResults { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<AnalyticsEvent> AnalyticsEvents { get; set; }
    
    // Matching Game
    public DbSet<MatchingQuestion> MatchingQuestions { get; set; }
    public DbSet<MatchingGameSession> MatchingGameSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Grade).IsRequired().HasMaxLength(50);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Question configuration
        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Text).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Difficulty).IsRequired();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Game configuration
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // GameQuestion configuration (junction table)
        modelBuilder.Entity<GameQuestion>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Game)
                .WithMany(g => g.GameQuestions)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Question)
                .WithMany(q => q.GameQuestions)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // TestResult configuration
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Student)
                .WithMany(s => s.TestResults)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Game)
                .WithMany(g => g.TestResults)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Admin configuration
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).HasConversion<string>();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired();
            
            entity.HasOne(e => e.Admin)
                .WithMany(a => a.RefreshTokens)
                .HasForeignKey(e => e.AdminId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Visit configuration
        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VisitorId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PagePath).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.VisitorId);
            entity.HasIndex(e => e.CreatedDate);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // AnalyticsEvent configuration
        modelBuilder.Entity<AnalyticsEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VisitorId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EventName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.EventName);
            entity.HasIndex(e => e.CreatedDate);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.CreatedDate);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // SystemSetting configuration
        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Key).IsUnique(); // Key must be unique
        });
    }
}
