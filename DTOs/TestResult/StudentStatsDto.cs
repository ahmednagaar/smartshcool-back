namespace Nafes.API.DTOs.TestResult;

public class StudentStatsDto
{
    public int TotalTests { get; set; }
    public double AverageScore { get; set; }
    public int TotalTimeSpentMinutes { get; set; }
    public int CurrentLevel { get; set; }
    public List<SubjectPerformanceDto> SubjectPerformance { get; set; } = new();
    public List<DailyActivityDto> WeeklyActivity { get; set; } = new();
}

public class SubjectPerformanceDto
{
    public string Subject { get; set; } = string.Empty;
    public double Score { get; set; }
}

public class DailyActivityDto
{
    public string Day { get; set; } = string.Empty;
    public int TestsCount { get; set; }
}
