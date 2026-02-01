using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules;

public class MatchingGameSession : BaseModel
{
    [Required]
    public long StudentId { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }

    public GradeLevel GradeId { get; set; }
    public SubjectType SubjectId { get; set; }
    
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectMatches { get; set; }
    public int TimeSpentSeconds { get; set; }
}
