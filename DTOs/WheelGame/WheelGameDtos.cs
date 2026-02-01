using Nafes.API.Modules;

namespace Nafes.API.DTOs.WheelGame;

public class StartWheelGameDto
{
    public long? StudentId { get; set; }
    public int GradeId { get; set; }
    public int SubjectId { get; set; }
    public int? NumberOfQuestions { get; set; }
    public DifficultyLevel? DifficultyLevel { get; set; }
    public TestType? TestType { get; set; }
}

public class SpinWheelDto
{
    public long SessionId { get; set; }
}

public class SpinResultDto
{
    public SegmentType SegmentType { get; set; }
    public int Value { get; set; }
    public string DisplayText { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
    public int RotationDegrees { get; set; }
    public string? SpecialEffect { get; set; }
}

public class SubmitAnswerDto
{
    public long SessionId { get; set; }
    public long QuestionId { get; set; }
    public string StudentAnswer { get; set; } = string.Empty;
    public SpinResultDto SpinResult { get; set; } = new();
    public int TimeSpent { get; set; }
    public bool HintUsed { get; set; }
}

public class AnswerResultDto
{
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int TotalScore { get; set; }
    public int QuestionsRemaining { get; set; }
    public bool SessionComplete { get; set; }
    public long? NextQuestionId { get; set; }
}

public class GetHintDto
{
    public long SessionId { get; set; }
    public long QuestionId { get; set; }
}

public class HintResponseDto
{
    public string HintText { get; set; } = string.Empty;
    public int PointsPenalty { get; set; }
}

public class SessionCompleteDto
{
    public long SessionId { get; set; }
    public int FinalScore { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public double Accuracy { get; set; }
    public int TimeSpent { get; set; }
    public int Rank { get; set; }
    public List<string> Achievements { get; set; } = new();
    public List<string> ImprovementTips { get; set; } = new();
}

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int Score { get; set; }
    public double Accuracy { get; set; }
    public int TimeSpent { get; set; }
    public DateTime DatePlayed { get; set; }
    public bool IsCurrentUser { get; set; }
}

public class StudentStatisticsDto
{
    public int TotalGamesPlayed { get; set; }
    public double AverageScore { get; set; }
    public int BestScore { get; set; }
    public int TotalCorrectAnswers { get; set; }
    public int TotalQuestionsAnswered { get; set; }
    public string FavoriteSubject { get; set; } = string.Empty;
    public double AverageTimePerGame { get; set; }
    public int HintsUsedTotal { get; set; }
}
