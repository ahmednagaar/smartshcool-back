using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Nafes.API.Modules;

namespace Nafes.API.DTOs.WheelGame;

public class CreateWheelQuestionDto
{
    [Required]
    [JsonPropertyName("gradeId")]
    public int GradeId { get; set; }

    [Required]
    [JsonPropertyName("subjectId")]
    public int SubjectId { get; set; }

    [Required]
    [JsonPropertyName("questionText")]
    public string QuestionText { get; set; } = string.Empty;

    [JsonPropertyName("questionType")]
    public QuestionType QuestionType { get; set; }

    [Required]
    [JsonPropertyName("correctAnswer")]
    public string CorrectAnswer { get; set; } = string.Empty;

    [JsonPropertyName("wrongAnswers")]
    public List<string> WrongAnswers { get; set; } = new();

    [JsonPropertyName("difficultyLevel")]
    public DifficultyLevel DifficultyLevel { get; set; }

    [JsonPropertyName("pointsValue")]
    public int? PointsValue { get; set; }

    [JsonPropertyName("timeLimit")]
    public int? TimeLimit { get; set; }

    [JsonPropertyName("hintText")]
    public string? HintText { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [JsonPropertyName("categoryTag")]
    public string? CategoryTag { get; set; }
    
    [JsonPropertyName("displayOrder")]
    public int? DisplayOrder { get; set; }
    
    [JsonPropertyName("testType")]
    public TestType? TestType { get; set; }
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
