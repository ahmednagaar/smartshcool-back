namespace Nafes.API.DTOs.Wheel;

public class WheelGameResultDto
{
    public long StudentId { get; set; }
    public int FinalScore { get; set; }
    public int QuestionsAnswered { get; set; }
    public int CorrectAnswers { get; set; }
    public int TimeSpentSeconds { get; set; }
}

public class WheelGameResultResponse
{
    public long Id { get; set; }
    public int FinalScore { get; set; }
    public int TotalPoints { get; set; }
    public List<string> NewAchievements { get; set; } = new();
}

public class WheelLeaderboardEntry
{
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public int HighScore { get; set; }
    public int TotalGames { get; set; }
    public int TotalPoints { get; set; }
}
