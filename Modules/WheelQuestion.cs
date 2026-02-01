using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules;



public class WheelQuestion : BaseModel
{
    [Required]
    public GradeLevel GradeId { get; set; }

    [Required]
    public SubjectType SubjectId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string QuestionText { get; set; } = string.Empty;

    public QuestionType QuestionType { get; set; }

    public TestType TestType { get; set; } = TestType.Nafes;

    [Required]
    [MaxLength(500)]
    public string CorrectAnswer { get; set; } = string.Empty;

    /// <summary>
    /// For MCQ: JSON array of incorrect options.
    /// </summary>
    public string WrongAnswers { get; set; } = "[]"; 

    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Easy;

    public int PointsValue { get; set; }

    public int TimeLimit { get; set; } = 30;

    [MaxLength(500)]
    public string? HintText { get; set; }

    public string? Explanation { get; set; }

    [MaxLength(100)]
    public string? CategoryTag { get; set; }
    
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
