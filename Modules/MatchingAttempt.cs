using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nafes.API.Modules;

public class MatchingAttempt
{
    [Key]
    public long Id { get; set; }

    public long SessionId { get; set; }
    
    [ForeignKey("SessionId")]
    [JsonIgnore]
    public MatchingGameSession Session { get; set; } = null!;

    public long PairId { get; set; }
    // Optional: Navigation to Pair if needed, but Session+PairId usually enough
    
    public bool IsCorrect { get; set; }

    public int PointsEarned { get; set; }

    public int TimeToMatchMs { get; set; }

    public DateTime AttemptTime { get; set; }
}
