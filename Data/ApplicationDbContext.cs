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
    public DbSet<MatchingGame> MatchingGames { get; set; }
    public DbSet<MatchingGamePair> MatchingGamePairs { get; set; }
    public DbSet<MatchingGameSession> MatchingGameSessions { get; set; }
    public DbSet<MatchingAttempt> MatchingAttempts { get; set; }

    // Wheel Game
    public DbSet<WheelQuestion> WheelQuestions { get; set; }
    public DbSet<WheelGameSession> WheelGameSessions { get; set; }
    public DbSet<WheelQuestionAttempt> WheelQuestionAttempts { get; set; }
    public DbSet<WheelSpinSegment> WheelSpinSegments { get; set; }


    // Drag & Drop Game
    public DbSet<DragDropQuestion> DragDropQuestions { get; set; }
    public DbSet<DragDropZone> DragDropZones { get; set; }
    public DbSet<DragDropItem> DragDropItems { get; set; }
    public DbSet<DragDropGameSession> DragDropGameSessions { get; set; }
    public DbSet<DragDropAttempt> DragDropAttempts { get; set; }

    // Flip Card Game
    public DbSet<FlipCardQuestion> FlipCardQuestions { get; set; }
    public DbSet<FlipCardPair> FlipCardPairs { get; set; }
    public DbSet<FlipCardGameSession> FlipCardGameSessions { get; set; }
    public DbSet<FlipCardAttempt> FlipCardAttempts { get; set; }

    // Passage Questions
    public DbSet<SubQuestion> SubQuestions { get; set; }

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
            entity.Property(e => e.PassageText).HasMaxLength(50000).IsRequired(false);
            entity.Property(e => e.EstimatedTimeMinutes).IsRequired(false);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // SubQuestion configuration
        modelBuilder.Entity<SubQuestion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Text).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Options).IsRequired();
            entity.Property(e => e.CorrectAnswer).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Explanation).HasMaxLength(2000);
            entity.Property(e => e.OrderIndex).IsRequired();
            entity.HasOne(sq => sq.Question)
                  .WithMany(q => q.SubQuestions)
                  .HasForeignKey(sq => sq.QuestionId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(sq => sq.QuestionId);
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
        // ... existing configurations ...
        });
        
        // Matching Game Configuration
        modelBuilder.Entity<MatchingGame>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.GradeId, e.SubjectId, e.IsActive });
            entity.HasMany(e => e.Pairs)
                .WithOne(p => p.MatchingGame)
                .HasForeignKey(p => p.MatchingGameId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MatchingGameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(s => s.MatchingGame)
                .WithMany()
                .HasForeignKey(s => s.MatchingGameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.Attempts)
                .WithOne(a => a.Session)
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MatchingAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SessionId);
        });

        // Wheel Game Configuration
        modelBuilder.Entity<WheelQuestion>()
            .HasIndex(q => new { q.GradeId, q.SubjectId, q.IsActive });

        modelBuilder.Entity<WheelGameSession>()
            .HasOne(s => s.Student)
            .WithMany()
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WheelQuestionAttempt>()
            .HasOne(a => a.Session)
            .WithMany(s => s.Attempts)
            .HasForeignKey(a => a.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<WheelQuestionAttempt>()
            .HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.NoAction); // Prevent cycles

        // Drag & Drop Configuration
        modelBuilder.Entity<DragDropQuestion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Grade, e.Subject, e.IsActive });
            entity.HasMany(e => e.Zones)
                .WithOne(z => z.DragDropQuestion)
                .HasForeignKey(z => z.DragDropQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Items)
                .WithOne(i => i.DragDropQuestion)
                .HasForeignKey(i => i.DragDropQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DragDropItem>(entity =>
        {
             entity.HasOne(e => e.CorrectZone)
                .WithMany()
                .HasForeignKey(e => e.CorrectZoneId)
                .OnDelete(DeleteBehavior.NoAction); // Zones are deleted by Question Cascade, avoid multiple cascade paths
        });

        modelBuilder.Entity<DragDropGameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.StudentId, e.IsCompleted });
            entity.HasMany(e => e.Attempts)
                .WithOne(a => a.Session)
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DragDropAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SessionId, e.ItemId });
            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cycles
             entity.HasOne(e => e.PlacedInZone)
                .WithMany()
                .HasForeignKey(e => e.PlacedInZoneId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cycles
        });

        // Flip Card Game Configuration
        modelBuilder.Entity<FlipCardQuestion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Grade, e.Subject, e.IsActive });
            entity.HasMany(e => e.Pairs)
                .WithOne(p => p.FlipCardQuestion)
                .HasForeignKey(p => p.FlipCardQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FlipCardGameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(s => s.FlipCardQuestion)
                .WithMany()
                .HasForeignKey(s => s.FlipCardQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.Attempts)
                .WithOne(a => a.Session)
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FlipCardAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SessionId);
        });
    }
}
