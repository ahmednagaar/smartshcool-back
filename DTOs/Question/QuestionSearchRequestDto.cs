using Nafes.API.Modules;

namespace Nafes.API.DTOs.Question;

public class QuestionSearchRequestDto
{
    public GradeLevel? Grade { get; set; }
    public SubjectType? Subject { get; set; }
    public DifficultyLevel? Difficulty { get; set; }
    public QuestionType? Type { get; set; }
    public TestType? TestType { get; set; }
    
    public string? SearchTerm { get; set; }
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
