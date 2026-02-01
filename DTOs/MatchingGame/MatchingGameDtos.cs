using System.ComponentModel.DataAnnotations;
using Nafes.API.Modules;

namespace Nafes.API.DTOs.MatchingGame;

public class CreateMatchingQuestionDto
{
    [Required]
    public GradeLevel GradeId { get; set; }

    [Required]
    public SubjectType SubjectId { get; set; }

    [Required]
    [MaxLength(200)]
    public string LeftItemText { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string RightItemText { get; set; } = string.Empty;

    [Required]
    public List<string> DistractorItems { get; set; } = new List<string>();

    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    
    public int DisplayOrder { get; set; }
}

public class UpdateMatchingQuestionDto : CreateMatchingQuestionDto
{
    public bool IsActive { get; set; }
}

public class MatchingQuestionDto
{
    public long Id { get; set; }
    public GradeLevel GradeId { get; set; }
    public SubjectType SubjectId { get; set; }
    public string LeftItemText { get; set; } = string.Empty;
    public string RightItemText { get; set; } = string.Empty;
    public List<string> DistractorItems { get; set; } = new List<string>();
    public DifficultyLevel DifficultyLevel { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class MatchingQuestionResponseDto
{
    public long Id { get; set; }
    public string LeftItem { get; set; } = string.Empty;
    // For student gameplay, we don't send the correct answer explicitly paired
    // Instead, we'll send a shuffled list of all right items + distractors separately
}

public class ShuffledMatchingGameDto
{
    public string SessionId { get; set; } = string.Empty; // Just for client ref if needed
    public List<MatchingPairDto> Pairs { get; set; } = new List<MatchingPairDto>();
}

public class MatchingPairDto 
{
    public long QuestionId { get; set; }
    public string LeftItem { get; set; } = string.Empty;
    // This part is tricky. Usually we want to send:
    // 1. A list of Left Items
    // 2. A list of Right Items (All correct answers + all distractors from all questions) mixed up
    // But keeping them somewhat grouped or purely pool-based depends on game design.
    // The prompt says: "Right column: Display all right items (answers + distractors) as drop zones"
    // So we need distinct lists.
}

public class GameStartResponseDto
{
    public long SessionId { get; set; }
    public List<GameLeftItemDto> LeftItems { get; set; } = new List<GameLeftItemDto>();
    public List<GameRightItemDto> RightItems { get; set; } = new List<GameRightItemDto>();
}

public class GameLeftItemDto
{
    public long Id { get; set; } // QuestionId
    public string Text { get; set; } = string.Empty;
    public string RightItemId { get; set; } = string.Empty; // For client-side validation
}

public class GameRightItemDto
{
    public string Id { get; set; } = string.Empty; // UUID to track which item this is
    public string Text { get; set; } = string.Empty;
}

public class StartGameDto
{
    public long StudentId { get; set; }
    public GradeLevel GradeId { get; set; }
    public SubjectType SubjectId { get; set; }
}

public class SubmitMatchingGameDto
{
    public long SessionId { get; set; }
    public List<ValidationMatchDto> Matches { get; set; } = new List<ValidationMatchDto>();
    public int TimeSpentSeconds { get; set; }
}

public class ValidationMatchDto
{
    public long QuestionId { get; set; }
    public string RightItemId { get; set; } = string.Empty;
}

public class GameResultDto
{
    public long SessionId { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectMatches { get; set; }
    public int TimeSpentSeconds { get; set; }
    public List<MatchResultDetailDto> Details { get; set; } = new List<MatchResultDetailDto>();
}

public class MatchResultDetailDto
{
    public long QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
}

public class MatchingLeaderboardDto
{
    public int Rank { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TimeSpent { get; set; }
    public DateTime DatePlayed { get; set; }
}
