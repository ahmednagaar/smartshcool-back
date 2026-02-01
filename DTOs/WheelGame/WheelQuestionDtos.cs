using System.ComponentModel.DataAnnotations;
using Nafes.API.Modules;

namespace Nafes.API.DTOs.WheelGame;

public class CreateWheelQuestionDto
{
    [Required]
    public int GradeId { get; set; }

    [Required]
    public int SubjectId { get; set; }

    [Required]
    [MinLength(10)]
    public string QuestionText { get; set; } = string.Empty;

    public QuestionType QuestionType { get; set; }

    [Required]
    public string CorrectAnswer { get; set; } = string.Empty;

    public List<string> WrongAnswers { get; set; } = new();

    public DifficultyLevel DifficultyLevel { get; set; }

    public int? PointsValue { get; set; }

    public int? TimeLimit { get; set; }

    public string? HintText { get; set; }

    public string? Explanation { get; set; }

    public string? CategoryTag { get; set; }
    
    public int? DisplayOrder { get; set; }
}

public class UpdateWheelQuestionDto : CreateWheelQuestionDto
{
    public bool IsActive { get; set; }
}

public class WheelQuestionResponseDto
{
    public long Id { get; set; }
    public int GradeId { get; set; }
    public int SubjectId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public List<string> Options { get; set; } = new(); // Shuffled
    public string CorrectAnswer { get; set; } = string.Empty;// Admin only or hidden?
    public DifficultyLevel DifficultyLevel { get; set; }
    public int PointsValue { get; set; }
    public int TimeLimit { get; set; }
    public string? HintText { get; set; }
    public string? CategoryTag { get; set; }
    public bool IsActive { get; set; }
}
