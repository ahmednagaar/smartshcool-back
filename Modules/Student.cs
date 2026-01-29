namespace Nafes.API.Modules;

public class Student : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Grade { get; set; } = string.Empty;
    
    // Unified Scoring
    public int TotalPoints { get; set; } = 0;
    public int WheelGamesPlayed { get; set; } = 0;
    public int TestsCompleted { get; set; } = 0;
    public int SpeedRoundsPlayed { get; set; } = 0;
    public DateTime? LastActiveAt { get; set; }

    // Navigation properties
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    public ICollection<StudentAchievement> StudentAchievements { get; set; } = new List<StudentAchievement>();
    public ICollection<WheelGameResult> WheelGameResults { get; set; } = new List<WheelGameResult>();
    
    // Authentication fields
    public string StudentCode { get; set; } = string.Empty; // e.g., "NAF-1234"
    public string? PinHash { get; set; } // 4-digit PIN, hashed
    public bool IsActive { get; set; } = true;
    
    // Parent relationship (optional - student may not have linked parent)
    public long? ParentId { get; set; }
    public Parent? Parent { get; set; }
}
