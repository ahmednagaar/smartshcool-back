namespace Nafes.API.DTOs.TestResult;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public int TestsCompleted { get; set; }
    public List<string> Badges { get; set; } = new();
}
