using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nafes.API.Modules;

public class MatchingGameSession : BaseModel
{
    [Required]
    public long StudentId { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }

    [Required]
    public long MatchingGameId { get; set; }

    [ForeignKey("MatchingGameId")]
    [JsonIgnore]
    public virtual MatchingGame MatchingGame { get; set; } = null!;

    public GradeLevel GradeId { get; set; }
    public SubjectType SubjectId { get; set; }
    
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    
    // Stats
    public int TotalScore { get; set; }
    public int TotalPairs { get; set; }
    public int MatchedPairs { get; set; }
    public int TotalMoves { get; set; }
    public int WrongAttempts { get; set; }
    public int TimeSpentSeconds { get; set; }
    public int HintsUsed { get; set; }
    public int StarRating { get; set; } // 0-3
    public bool IsCompleted { get; set; }

    public ICollection<MatchingAttempt> Attempts { get; set; } = new List<MatchingAttempt>();
}
