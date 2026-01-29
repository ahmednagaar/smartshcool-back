namespace Nafes.API.DTOs.Parent;

public class ParentRegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ChildStudentCode { get; set; } = string.Empty; // Link to existing child
}

public class ParentLoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ParentLoginResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public List<ChildInfoDto> Children { get; set; } = new();
}

public class ChildInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StudentCode { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
}

public class ChildProgressDto
{
    public long StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string StudentCode { get; set; } = string.Empty;
    public int TotalTests { get; set; }
    public double AverageScore { get; set; }
    public int TotalAchievements { get; set; }
    public int LeaderboardRank { get; set; }
    public List<RecentTestDto> RecentTests { get; set; } = new();
    public List<AchievementSummaryDto> Achievements { get; set; } = new();
}

public class RecentTestDto
{
    public string GameTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public bool Passed { get; set; }
    public DateTime DateTaken { get; set; }
}

public class AchievementSummaryDto
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime DateUnlocked { get; set; }
}
