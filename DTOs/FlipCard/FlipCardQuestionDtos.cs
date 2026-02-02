using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nafes.API.Modules;

namespace Nafes.API.DTOs.FlipCard
{
    public class FlipCardQuestionDto
    {
        public int Id { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; } = string.Empty; // Mapped manually if needed? Or AutoMapper can flat map if property exists on Entity.Entity.Name
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string GameTitle { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public FlipCardGameMode GameMode { get; set; }
        public int NumberOfPairs { get; set; }
        public List<FlipCardPairDto> Pairs { get; set; } = new();
        public DifficultyLevel DifficultyLevel { get; set; }
        public FlipCardTimerMode TimerMode { get; set; }
        public int? TimeLimitSeconds { get; set; }
        public int PointsPerMatch { get; set; }
        public int MovePenalty { get; set; }
        public bool ShowHints { get; set; }
        public int MaxHints { get; set; }
        public string UITheme { get; set; } = "modern";
        public string CardBackDesign { get; set; } = "pattern1";
        public string? CustomCardBackUrl { get; set; }
        public bool EnableAudio { get; set; }
        public bool EnableExplanations { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class FlipCardPairDto
    {
        public int Id { get; set; }
        public int FlipCardQuestionId { get; set; }
        
        public FlipCardContentType Card1Type { get; set; }
        public string? Card1Text { get; set; }
        public string? Card1ImageUrl { get; set; }
        public string? Card1AudioUrl { get; set; }
        
        public FlipCardContentType Card2Type { get; set; }
        public string? Card2Text { get; set; }
        public string? Card2ImageUrl { get; set; }
        public string? Card2AudioUrl { get; set; }
        
        public string? Explanation { get; set; }
        public int PairOrder { get; set; }
        public int DifficultyWeight { get; set; }
    }

    public class CreateFlipCardQuestionDto
    {
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public string GameTitle { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public FlipCardGameMode GameMode { get; set; }
        public int NumberOfPairs { get; set; }
        public List<CreateFlipCardPairDto> Pairs { get; set; } = new();
        public DifficultyLevel DifficultyLevel { get; set; }
        public FlipCardTimerMode TimerMode { get; set; }
        public int? TimeLimitSeconds { get; set; }
        public int PointsPerMatch { get; set; }
        public int MovePenalty { get; set; }
        public bool ShowHints { get; set; }
        public int MaxHints { get; set; }
        public string UITheme { get; set; } = "modern";
        public string CardBackDesign { get; set; } = "pattern1";
        public string? CustomCardBackUrl { get; set; }
        public bool EnableAudio { get; set; }
        public bool EnableExplanations { get; set; }
        public string Category { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }

    public class UpdateFlipCardQuestionDto : CreateFlipCardQuestionDto
    {
        public int Id { get; set; }
    }

    public class CreateFlipCardPairDto
    {
        public FlipCardContentType Card1Type { get; set; }
        public string? Card1Text { get; set; }
        public string? Card1ImageUrl { get; set; }
        public string? Card1AudioUrl { get; set; }
        
        public FlipCardContentType Card2Type { get; set; }
        public string? Card2Text { get; set; }
        public string? Card2ImageUrl { get; set; }
        public string? Card2AudioUrl { get; set; }
        
        public string? Explanation { get; set; }
        public int? DifficultyWeight { get; set; }
        public int PairOrder { get; set; } // Added Order
    }
}
