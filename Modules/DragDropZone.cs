using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nafes.API.Modules
{
    public class DragDropZone
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DragDropQuestionId { get; set; }
        
        [JsonIgnore]
        public DragDropQuestion DragDropQuestion { get; set; }

        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ColorCode { get; set; } = "#4CAF50"; // Fallback/Default

        public int ZoneOrder { get; set; }

        [MaxLength(255)]
        public string? IconUrl { get; set; }
    }
}
