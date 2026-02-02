using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nafes.API.Modules
{
    public class DragDropItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DragDropQuestionId { get; set; }
        
        [JsonIgnore]
        public DragDropQuestion? DragDropQuestion { get; set; }

        [Required]
        [MaxLength(200)]
        public string Text { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? AudioUrl { get; set; }

        [Required]
        public int CorrectZoneId { get; set; }
        
        [JsonIgnore]
        public DragDropZone? CorrectZone { get; set; }

        public int ItemOrder { get; set; }

        [MaxLength(500)]
        public string? Explanation { get; set; }
    }
}
