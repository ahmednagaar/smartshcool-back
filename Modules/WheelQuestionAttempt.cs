using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules;

public class WheelQuestionAttempt : BaseModel
{
    public long SessionId { get; set; }
    [ForeignKey("SessionId")]
    public virtual WheelGameSession Session { get; set; } = null!;

    public long QuestionId { get; set; }
    [ForeignKey("QuestionId")]
    public virtual WheelQuestion Question { get; set; } = null!;

    [MaxLength(500)]
    public string StudentAnswer { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public int TimeSpent { get; set; }
    public bool HintUsed { get; set; }
    
    [MaxLength(50)]
    public string SpinResult { get; set; } = string.Empty;

    public DateTime AttemptTime { get; set; } = DateTime.UtcNow;
}
