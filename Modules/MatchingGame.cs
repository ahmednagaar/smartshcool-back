using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules;

public class MatchingGame : BaseModel
{
    [Required]
    [MaxLength(500)]
    public string GameTitle { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Instructions { get; set; } = string.Empty;

    [Required]
    public GradeLevel GradeId { get; set; }

    [Required]
    public SubjectType SubjectId { get; set; }

    [Required]
    [Range(4, 12)]
    public int NumberOfPairs { get; set; } // 4, 6, 8, 10, 12 pairs

    public MatchingMode MatchingMode { get; set; }

    [MaxLength(50)]
    public string UITheme { get; set; } = "modern";

    public bool ShowConnectingLines { get; set; }

    public bool EnableAudio { get; set; }

    public bool EnableHints { get; set; }
    public int MaxHints { get; set; }

    public MatchingTimerMode TimerMode { get; set; }
    public int? TimeLimitSeconds { get; set; }

    public int PointsPerMatch { get; set; } = 100;
    public int WrongMatchPenalty { get; set; } = 5;

    public DifficultyLevel DifficultyLevel { get; set; }

    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    public List<MatchingGamePair> Pairs { get; set; } = new();

    public string CreatedBy { get; set; } = string.Empty; // Username or ID
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}

public enum MatchingMode
{
    ClickToClick = 0,
    DragDrop = 1,
    Both = 2
}

public enum MatchingTimerMode
{
    None = 0,
    CountUp = 1,
    Countdown = 2
}
