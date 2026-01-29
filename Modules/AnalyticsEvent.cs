using System.ComponentModel.DataAnnotations;

namespace Nafes.API.Modules;

public class AnalyticsEvent : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string VisitorId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EventName { get; set; } = string.Empty; // e.g., "select_test_type", "start_game"

    public string? EventProperties { get; set; } // JSON string of properties
    
    public string? PagePath { get; set; }
}
