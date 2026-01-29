namespace Nafes.API.DTOs.Analytics;

public class ActivityTrendDto
{
    public string Label { get; set; } = string.Empty; // Date label (e.g., "Jan 1")
    public int GamesPlayed { get; set; }
    public int ActiveStudents { get; set; }
}

public class ActivityTrendsResponseDto
{
    public List<string> Labels { get; set; } = new();
    public List<ActivityDatasetDto> Datasets { get; set; } = new();
}

public class ActivityDatasetDto
{
    public string Label { get; set; } = string.Empty;
    public List<int> Data { get; set; } = new();
}

public class DifficultQuestionDto
{
    public long Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public double ErrorRate { get; set; } // 0.0 to 100.0
    public int Attempts { get; set; }
    public int IncorrectCount { get; set; }
}

public class EngagementSummaryDto
{
    public int DailyActiveUsers { get; set; }
    public string? AvgSessionDuration { get; set; } // Format "12:34"
    public int GamesPlayedToday { get; set; }
    public string? PopularGameMode { get; set; }
    public int TotalStudents { get; set; }
}
