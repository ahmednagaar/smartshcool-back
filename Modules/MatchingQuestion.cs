using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules;

public class MatchingQuestion : BaseModel
{
    [Required]
    public GradeLevel GradeId { get; set; }

    [Required]
    public SubjectType SubjectId { get; set; }

    [Required]
    [MaxLength(200)]
    public string LeftItemText { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string RightItemText { get; set; } = string.Empty;

    /// <summary>
    /// stored as JSON array of strings
    /// </summary>
    [Required]
    public string DistractorItems { get; set; } = "[]";

    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;

    public int DisplayOrder { get; set; }
    
    public bool IsActive { get; set; } = true;
}
