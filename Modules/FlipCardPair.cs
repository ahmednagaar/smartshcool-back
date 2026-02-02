using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nafes.API.Modules
{
    public class FlipCardPair
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FlipCardQuestionId { get; set; }
        
        [ForeignKey("FlipCardQuestionId")]
        [JsonIgnore]
        public FlipCardQuestion FlipCardQuestion { get; set; } = null!;
        
        // Card 1
        [Required]
        public FlipCardContentType Card1Type { get; set; }
        
        [MaxLength(500)]
        public string? Card1Text { get; set; }
        
        [MaxLength(500)]
        public string? Card1ImageUrl { get; set; }
        
        [MaxLength(500)]
        public string? Card1AudioUrl { get; set; }
        
        // Card 2
        [Required]
        public FlipCardContentType Card2Type { get; set; }
        
        [MaxLength(500)]
        public string? Card2Text { get; set; }
        
        [MaxLength(500)]
        public string? Card2ImageUrl { get; set; }
        
        [MaxLength(500)]
        public string? Card2AudioUrl { get; set; }
        
        [MaxLength(1000)]
        public string? Explanation { get; set; }
        
        public int PairOrder { get; set; }
        
        public int DifficultyWeight { get; set; } = 5;
    }

    public enum FlipCardContentType
    {
        Text = 0,
        Image = 1,
        Audio = 2,
        TextAndImage = 3,
        TextAndAudio = 4,
        Mixed = 5
    }
}
