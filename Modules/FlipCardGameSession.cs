using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules
{
    public class FlipCardGameSession
    {
        [Key]
        public int Id { get; set; }
        
        public long StudentId { get; set; }
        
        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;
        
        public int FlipCardQuestionId { get; set; }
        
        [ForeignKey("FlipCardQuestionId")]
        public FlipCardQuestion FlipCardQuestion { get; set; } = null!;
        
        public GradeLevel Grade { get; set; }
        public SubjectType Subject { get; set; }
        
        public FlipCardGameMode GameMode { get; set; }
        
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        
        public int TotalPairs { get; set; }
        public int MatchedPairs { get; set; }
        public int TotalMoves { get; set; }
        public int WrongAttempts { get; set; }
        
        public int TotalScore { get; set; }
        public int TimeSpentSeconds { get; set; }
        
        public int HintsUsed { get; set; }
        
        public bool IsCompleted { get; set; }
        
        public int StarRating { get; set; }
        
        public List<FlipCardAttempt> Attempts { get; set; } = new();
    }
}
