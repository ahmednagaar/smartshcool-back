using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules
{
    public class DragDropAttempt
    {
        [Key]
        public int Id { get; set; }

        public int SessionId { get; set; }
        public DragDropGameSession Session { get; set; }

        public int ItemId { get; set; }
        public DragDropItem Item { get; set; }

        public int PlacedInZoneId { get; set; }
        public DragDropZone PlacedInZone { get; set; }

        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }

        public DateTime AttemptTime { get; set; } = DateTime.UtcNow;
    }
}
