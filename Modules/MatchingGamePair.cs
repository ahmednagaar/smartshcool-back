using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nafes.API.Modules;

public class MatchingGamePair
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long MatchingGameId { get; set; }
    
    [ForeignKey("MatchingGameId")]
    [JsonIgnore]
    public MatchingGame MatchingGame { get; set; } = null!;

    // Question (Left)
    [Required]
    [MaxLength(500)]
    public string QuestionText { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? QuestionImageUrl { get; set; }

    [MaxLength(500)]
    public string? QuestionAudioUrl { get; set; }

    public MatchingContentType QuestionType { get; set; }

    // Answer (Right)
    [Required]
    [MaxLength(500)]
    public string AnswerText { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? AnswerImageUrl { get; set; }

    [MaxLength(500)]
    public string? AnswerAudioUrl { get; set; }

    public MatchingContentType AnswerType { get; set; }

    [MaxLength(1000)]
    public string? Explanation { get; set; }

    public int PairOrder { get; set; }
}

public enum MatchingContentType
{
    Text = 0,
    Image = 1,
    TextAndImage = 2,
    Audio = 3,
    Mixed = 4
}
