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
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? AdminNotes { get; set; }
    public List<string>? Tags { get; set; }
}

public class QuestionCreateDto
{
    [Required(ErrorMessage = "نص السؤال مطلوب")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "يجب أن يكون السؤال بين 10 و 500 حرف")]
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

    [ValidateJsonArray(minCount: 0, maxCount: 10, ErrorMessage = "الخيارات غير صالحة")]
    public string? Options { get; set; }

    [Required(ErrorMessage = "الإجابة الصحيحة مطلوبة")]
    [StringLength(500)]
    public string CorrectAnswer { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? AdminNotes { get; set; }
    
    public List<string>? Tags { get; set; }
}

public class QuestionUpdateDto
{
    [StringLength(500, MinimumLength = 10, ErrorMessage = "يجب أن يكون السؤال بين 10 و 500 حرف")]
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
