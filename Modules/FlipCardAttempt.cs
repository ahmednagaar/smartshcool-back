using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nafes.API.Modules
{
    public class FlipCardAttempt
    {
        [Key]
        public int Id { get; set; }
        
        public int SessionId { get; set; }
        
        [ForeignKey("SessionId")]
        [JsonIgnore]
        public FlipCardGameSession Session { get; set; } = null!;
        
        public int PairId { get; set; } // Can relate to FlipCardPair if needed, but keeping it loosely coupled by ID is fine for strict analytics
        
        public int Card1FlippedAtMs { get; set; }
        public int Card2FlippedAtMs { get; set; }
        
        public bool IsCorrectMatch { get; set; }
        
        public int PointsEarned { get; set; }
        
        public int AttemptsBeforeMatch { get; set; }
        
        public DateTime AttemptTime { get; set; } = DateTime.UtcNow;
    }
}
