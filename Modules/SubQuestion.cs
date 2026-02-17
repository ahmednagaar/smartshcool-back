using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules;

public class SubQuestion : BaseModel
{
    [Required]
    public long QuestionId { get; set; }
    
    [ForeignKey(nameof(QuestionId))]
    public Question Question { get; set; } = null!;
    
    [Required]
    public int OrderIndex { get; set; }
    
    [Required]
    [StringLength(1000)]
    public string Text { get; set; } = string.Empty;
    
    [Required]
    public string Options { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500)]
    public string CorrectAnswer { get; set; } = string.Empty;
    
    [StringLength(2000)]
    public string? Explanation { get; set; }
}
