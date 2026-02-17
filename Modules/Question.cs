namespace Nafes.API.Modules;

public enum QuestionType
{
    MultipleChoice = 1,
    TrueFalse = 2,
    ConnectLines = 3,
    FillInTheBlank = 4,
    DragDrop = 5,
    Passage = 6
}

public enum DifficultyLevel
{
    Easy = 1,
    Medium = 2,
    Hard = 3
}

public enum GradeLevel
{
    Grade3 = 3,
    Grade4 = 4,
    Grade5 = 5,
    Grade6 = 6
}

public enum SubjectType
{
    Arabic = 1,
    Math = 2,
    Science = 3
}

public enum TestType
{
    Nafes = 1,
    Central = 2
}

public class Question : BaseModel
{
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string? MediaUrl { get; set; }
    
    // New fields for educational platform flow
    public GradeLevel Grade { get; set; } = GradeLevel.Grade3;
    public SubjectType Subject { get; set; } = SubjectType.Math;
    public TestType TestType { get; set; } = TestType.Central;
    
    // Passage question fields
    public string? PassageText { get; set; }
    public int? EstimatedTimeMinutes { get; set; }
    
    // For multiple choice - stored as JSON
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    
    // Navigation properties
    public ICollection<GameQuestion> GameQuestions { get; set; } = new List<GameQuestion>();
    public ICollection<SubQuestion> SubQuestions { get; set; } = new List<SubQuestion>();
}

