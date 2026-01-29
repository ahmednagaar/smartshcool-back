using System.ComponentModel.DataAnnotations;

namespace Nafes.API.Modules;

public class Visit : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string VisitorId { get; set; } = string.Empty; // UUID from frontend local storage

    [Required]
    [MaxLength(200)]
    public string PagePath { get; set; } = string.Empty;

    public string? UserAgent { get; set; }
    
    public string? DeviceType { get; set; } // Mobile, Desktop, Tablet
    
    public string? Source { get; set; } // e.g., Direct, Referral
}
