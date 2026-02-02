using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules
{
    public class FlipCardQuestion
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public GradeLevel Grade { get; set; }
        
        [Required]
        public SubjectType Subject { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string GameTitle { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Instructions { get; set; }
        
        [Required]
        public FlipCardGameMode GameMode { get; set; }
        
        [Required]
        [Range(4, 12)]
        public int NumberOfPairs { get; set; }
        
        public List<FlipCardPair> Pairs { get; set; } = new();
        
        public DifficultyLevel DifficultyLevel { get; set; }
        
        public FlipCardTimerMode TimerMode { get; set; }
        
        public int? TimeLimitSeconds { get; set; }
        
        public int PointsPerMatch { get; set; } = 100;
        
        public int MovePenalty { get; set; } = 5;
        
        public bool ShowHints { get; set; } = true;
        
        public int MaxHints { get; set; } = 2;
        
        [Required]
        [MaxLength(50)]
        public string UITheme { get; set; } = "modern";
        
        [MaxLength(50)]
        public string CardBackDesign { get; set; } = "pattern1";
        
        [MaxLength(500)]
        public string? CustomCardBackUrl { get; set; }
        
        public bool EnableAudio { get; set; } = true;
        
        public bool EnableExplanations { get; set; } = true;
        
        [MaxLength(100)]
        public string? Category { get; set; }
        
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
    }

    public enum FlipCardGameMode
    {
        ClassicMemory = 0,
        MatchMode = 1,
        Both = 2
    }

    public enum FlipCardTimerMode
    {
        None = 0,
        CountUp = 1,
        Countdown = 2
    }
}
