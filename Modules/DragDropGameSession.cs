using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules
{
    public class DragDropGameSession
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }
        // public Student Student { get; set; } // Assuming Student entity exists

        public int DragDropQuestionId { get; set; }
        public DragDropQuestion DragDropQuestion { get; set; }

        public GradeLevel Grade { get; set; }
        public SubjectType Subject { get; set; }

        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }

        public int TotalItems { get; set; }
        public int CorrectPlacements { get; set; }
        public int WrongPlacements { get; set; }

        public int TotalScore { get; set; }
        public int TimeSpentSeconds { get; set; }

        public bool IsCompleted { get; set; }

        public List<DragDropAttempt> Attempts { get; set; } = new();
    }
}
