using System.ComponentModel.DataAnnotations;
using Nafes.API.Modules;
using Nafes.API.Validation;

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
    
    // Passage fields
    public string? PassageText { get; set; }
    public int? EstimatedTimeMinutes { get; set; }
    public List<SubQuestionGetDto>? SubQuestions { get; set; }
    
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? AdminNotes { get; set; }
    public List<string>? Tags { get; set; }
}

public class SubQuestionGetDto
{
    public long Id { get; set; }
    public long QuestionId { get; set; }
    public int OrderIndex { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Options { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
}

public class QuestionCreateDto
{
    [Required(ErrorMessage = "نص السؤال مطلوب")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "يجب أن يكون السؤال بين 5 و 500 حرف")]
    public string Text { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع السؤال مطلوب")]
    [EnumDataType(typeof(QuestionType))]
    public QuestionType Type { get; set; }

    [Required(ErrorMessage = "مستوى الصعوبة مطلوب")]
    [EnumDataType(typeof(DifficultyLevel))]
    public DifficultyLevel Difficulty { get; set; }

    [Required(ErrorMessage = "الصف الدراسي مطلوب")]
    [Range(3, 6, ErrorMessage = "الصفوف المتاحة من 3 إلى 6 فقط")]
    public GradeLevel Grade { get; set; }

    [Required(ErrorMessage = "المادة مطلوبة")]
    [EnumDataType(typeof(SubjectType))]
    public SubjectType Subject { get; set; }

    [Required(ErrorMessage = "نوع الاختبار مطلوب")]
    public TestType TestType { get; set; }

    [StringLength(2048)]
    public string? MediaUrl { get; set; }
    
    // Passage fields
    [StringLength(50000, ErrorMessage = "نص القطعة يجب أن لا يتجاوز 50000 حرف")]
    public string? PassageText { get; set; }
    
    [Range(1, 120, ErrorMessage = "الوقت المقدر يجب أن يكون بين 1 و 120 دقيقة")]
    public int? EstimatedTimeMinutes { get; set; }
    
    public List<SubQuestionCreateDto>? SubQuestions { get; set; }

    // Normal question fields (not used for Passage type)
    [ValidateJsonArray(minCount: 0, maxCount: 10, ErrorMessage = "الخيارات غير صالحة")]
    public string? Options { get; set; }

    [StringLength(500)]
    public string? CorrectAnswer { get; set; }
    
    [StringLength(1000)]
    public string? AdminNotes { get; set; }
    
    public List<string>? Tags { get; set; }
}

public class SubQuestionCreateDto
{
    [Required]
    [Range(1, 20)]
    public int OrderIndex { get; set; }
    
    [Required(ErrorMessage = "نص السؤال الفرعي مطلوب")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "نص السؤال الفرعي يجب أن يكون بين 5 و 1000 حرف")]
    public string Text { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "خيارات الإجابة مطلوبة")]
    public string Options { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "الإجابة الصحيحة مطلوبة")]
    [StringLength(500)]
    public string CorrectAnswer { get; set; } = string.Empty;
    
    [StringLength(2000)]
    public string? Explanation { get; set; }
}

public class QuestionUpdateDto
{
    [StringLength(500, MinimumLength = 5, ErrorMessage = "يجب أن يكون السؤال بين 5 و 500 حرف")]
    public string Text { get; set; } = string.Empty;
    
    [EnumDataType(typeof(QuestionType))]
    public QuestionType Type { get; set; }
    
    [EnumDataType(typeof(DifficultyLevel))]
    public DifficultyLevel Difficulty { get; set; }
    
    [Range(3, 6, ErrorMessage = "الصفوف المتاحة من 3 إلى 6 فقط")]
    public GradeLevel Grade { get; set; }
    
    [EnumDataType(typeof(SubjectType))]
    public SubjectType Subject { get; set; }
    
    public TestType TestType { get; set; }
    
    [StringLength(2048)]
    public string? MediaUrl { get; set; }
    
    // Passage fields
    [StringLength(50000)]
    public string? PassageText { get; set; }
    
    [Range(1, 120)]
    public int? EstimatedTimeMinutes { get; set; }
    
    public List<SubQuestionCreateDto>? SubQuestions { get; set; }
    
    [ValidateJsonArray(minCount: 0, maxCount: 10)]
    public string? Options { get; set; }
    
    [StringLength(500)]
    public string? CorrectAnswer { get; set; }
    
    [StringLength(1000)]
    public string? AdminNotes { get; set; }
    
    public List<string>? Tags { get; set; }
}

public class QuestionSearchRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public QuestionType? Type { get; set; }
    public DifficultyLevel? Difficulty { get; set; }
    public GradeLevel? Grade { get; set; }
    public SubjectType? Subject { get; set; }
    public TestType? TestType { get; set; }
    public string? SortBy { get; set; } = "CreatedDate";
    public string? SortOrder { get; set; } = "desc";
}

public class BulkImportResultDto
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<ImportFailure> Failures { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public class ImportFailure
{
    public int Row { get; set; }
    public List<string> Errors { get; set; } = new();
}
