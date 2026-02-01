using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules;

public class WheelGameSession : BaseModel
{
    public long? StudentId { get; set; }
    public virtual Student? Student { get; set; }

    public GradeLevel GradeId { get; set; }
    public SubjectType SubjectId { get; set; }

    public int TotalQuestions { get; set; }
    public int QuestionsAnswered { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int TotalScore { get; set; }
    public int HintsUsed { get; set; }

    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    
    public bool IsCompleted { get; set; }
    public int TimeSpentSeconds { get; set; }

    /// <summary>
    /// Stored as JSON: includes question order, spin results, active effects.
    /// </summary>
    public string SessionData { get; set; } = "{}";

    public virtual ICollection<WheelQuestionAttempt> Attempts { get; set; } = new List<WheelQuestionAttempt>();
}
