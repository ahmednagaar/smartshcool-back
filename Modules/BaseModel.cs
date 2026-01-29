using System.ComponentModel.DataAnnotations;

namespace Nafes.API.Modules;

public abstract class BaseModel
{
    [Key]
    public long Id { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public string CreatedBy { get; set; } = string.Empty;
    
    public DateTime? UpdatedDate { get; set; }
    
    public string UpdatedBy { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; } = false;
    
    public DateTime? DeletedAt { get; set; }
}
