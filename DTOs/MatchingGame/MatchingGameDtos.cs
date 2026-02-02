using System.ComponentModel.DataAnnotations;
using Nafes.API.Modules;

namespace Nafes.API.DTOs.Matching;

public class CreateMatchingGameDto
{
    [Required]
    [MaxLength(500)]
    public string GameTitle { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public GradeLevel GradeId { get; set; }
    public SubjectType SubjectId { get; set; }
    public int NumberOfPairs { get; set; }
    public MatchingMode MatchingMode { get; set; }
    public string UITheme { get; set; } = "modern";
    public bool ShowConnectingLines { get; set; }
    public bool EnableAudio { get; set; }
    public bool EnableHints { get; set; }
    public int MaxHints { get; set; }
    public MatchingTimerMode TimerMode { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public int PointsPerMatch { get; set; }
    public int WrongMatchPenalty { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<CreateMatchingGamePairDto> Pairs { get; set; } = new();
}

public class UpdateMatchingGameDto : CreateMatchingGameDto
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateMatchingGamePairDto
{
    // Question
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public string? QuestionAudioUrl { get; set; }
    public MatchingContentType QuestionType { get; set; }
    
    // Answer
    public string AnswerText { get; set; } = string.Empty;
    public string? AnswerImageUrl { get; set; }
    public string? AnswerAudioUrl { get; set; }
    public MatchingContentType AnswerType { get; set; }
    
    public string? Explanation { get; set; }
}

public class MatchingGameDto
{
    public long Id { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public GradeLevel GradeId { get; set; }
    public SubjectType SubjectId { get; set; }
    public int NumberOfPairs { get; set; }
    public MatchingMode MatchingMode { get; set; }
    public string UITheme { get; set; }
    public bool ShowConnectingLines { get; set; }
    public bool EnableAudio { get; set; }
    public bool EnableHints { get; set; }
    public int MaxHints { get; set; }
    public MatchingTimerMode TimerMode { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public int PointsPerMatch { get; set; }
    public int WrongMatchPenalty { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedDate { get; set; }
    
    public List<MatchingGamePairDto> Pairs { get; set; } = new();
}

public class MatchingGamePairDto
{
    public long Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public string? QuestionAudioUrl { get; set; }
    public MatchingContentType QuestionType { get; set; }
    
    public string AnswerText { get; set; } = string.Empty;
    public string? AnswerImageUrl { get; set; }
    public string? AnswerAudioUrl { get; set; }
    public MatchingContentType AnswerType { get; set; }
    
    public string? Explanation { get; set; }
    public int PairOrder { get; set; }
}

// Student DTOs
public class StartMatchingGameDto
{
    public long? StudentId { get; set; }
    public GradeLevel GradeId { get; set; }
    public SubjectType SubjectId { get; set; }
    public int? GameId { get; set; }
    public DifficultyLevel? DifficultyLevel { get; set; }
}

public class MatchingGameStartResponseDto
{
    public long SessionId { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int NumberOfPairs { get; set; }
    public MatchingMode MatchingMode { get; set; }
    public string UITheme { get; set; } = "modern";
    public bool ShowConnectingLines { get; set; }
    public bool EnableAudio { get; set; }
    public bool EnableHints { get; set; }
    public int MaxHints { get; set; }
    public MatchingTimerMode TimerMode { get; set; }
    public int? TimeLimitSeconds { get; set; }
    
    public List<GameCardDto> Questions { get; set; } = new();
    public List<GameCardDto> Answers { get; set; } = new();
}

public class GameCardDto
{
    public long Id { get; set; } // PairId
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public MatchingContentType Type { get; set; }
}

public class ValidateMatchDto
{
    public long SessionId { get; set; }
    public long QuestionId { get; set; } // PairId
    public long AnswerId { get; set; } // PairId
    public int TimeToMatchMs { get; set; }
}

public class MatchResultDto
{
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public int TotalScore { get; set; }
    public int MatchedPairs { get; set; }
    public int TotalPairs { get; set; }
    public string? Explanation { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsGameComplete { get; set; }
}

public class GetHintDto
{
    public long SessionId { get; set; }
}

public class HintResponseDto
{
    public long QuestionId { get; set; } // Should be long
    public long AnswerId { get; set; } // Should be long
    public int HintsRemaining { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class SessionCompleteDto
{
    public long SessionId { get; set; }
    public int FinalScore { get; set; }
    public int MatchedPairs { get; set; }
    public int TotalPairs { get; set; }
    public int TotalMoves { get; set; }
    public int WrongAttempts { get; set; }
    public int TimeSpent { get; set; }
    public int StarRating { get; set; } // 1-3
    public int Rank { get; set; }
    public List<string> Achievements { get; set; } = new();
}

public class MatchingLeaderboardDto
{
    public int Rank { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TimeSpent { get; set; }
    public DateTime DatePlayed { get; set; }
}
