using Nafes.API.Modules;

namespace Nafes.API.DTOs.Question;

public class QuestionGetDto
{
    public long Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public string DifficultyName { get; set; } = string.Empty;
    public GradeLevel Grade { get; set; }
    public SubjectType Subject { get; set; }
    public TestType TestType { get; set; }
    public string? MediaUrl { get; set; }
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class QuestionCreateDto
{
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public GradeLevel Grade { get; set; }
    public SubjectType Subject { get; set; }
    public TestType TestType { get; set; }
    public string? MediaUrl { get; set; }
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
}

public class QuestionUpdateDto
{
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public GradeLevel Grade { get; set; }
    public SubjectType Subject { get; set; }
    public TestType TestType { get; set; }
    public string? MediaUrl { get; set; }
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
}

