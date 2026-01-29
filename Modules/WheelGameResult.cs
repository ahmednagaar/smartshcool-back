namespace Nafes.API.Modules;

public class WheelGameResult : BaseModel
{
    public long StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public int FinalScore { get; set; }
    public int QuestionsAnswered { get; set; }
    public int CorrectAnswers { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
}
