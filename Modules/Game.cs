namespace Nafes.API.Modules;

public class Game : BaseModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TimeLimit { get; set; } // in minutes
    public int PassingScore { get; set; } // percentage
    
    // New fields for game specificity
    public GradeLevel Grade { get; set; } = GradeLevel.Grade3;
    public SubjectType Subject { get; set; } = SubjectType.Math;
    
    // Navigation properties
    public ICollection<GameQuestion> GameQuestions { get; set; } = new List<GameQuestion>();
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}

// Junction table for many-to-many relationship
public class GameQuestion : BaseModel
{
    public long GameId { get; set; }
    public Game Game { get; set; } = null!;
    
    public long QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    
    public int Order { get; set; } // Question order in the game
}
