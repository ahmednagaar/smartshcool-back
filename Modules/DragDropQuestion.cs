using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules
{
    public class DragDropQuestion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public GradeLevel Grade { get; set; }

        [Required]
        public SubjectType Subject { get; set; }

        [Required]
        [MaxLength(500)]
        public string GameTitle { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Instructions { get; set; }

        [Required]
        [Range(2, 5)]
        public int NumberOfZones { get; set; }

        public List<DragDropZone> Zones { get; set; } = new();

        public List<DragDropItem> Items { get; set; } = new();

        public DifficultyLevel DifficultyLevel { get; set; }

        public int? TimeLimit { get; set; } // Seconds

        public int PointsPerCorrectItem { get; set; } = 10;

        public bool ShowImmediateFeedback { get; set; } = true;

        [Required]
        [MaxLength(50)]
        public string UITheme { get; set; } = "modern";

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }
    }
}
