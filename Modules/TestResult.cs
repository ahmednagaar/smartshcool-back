namespace Nafes.API.Modules;

public class TestResult : BaseModel
{
    public long StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public long GameId { get; set; }
    public Game Game { get; set; } = null!;
    
    public int Score { get; set; } // Percentage or points
    public DateTime DateTaken { get; set; } = DateTime.UtcNow;
    public int TimeSpent { get; set; } // in minutes
    
    // Store answers as JSON
    public string Answers { get; set; } = string.Empty;
    
    public bool Passed { get; set; }
}
