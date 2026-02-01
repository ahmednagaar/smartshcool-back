using System.ComponentModel.DataAnnotations;
using Nafes.API.Modules;

namespace Nafes.API.DTOs.WheelGame;

public class CreateSegmentDto
{
    public SegmentType SegmentType { get; set; }
    public int SegmentValue { get; set; }
    [Required]
    public string DisplayText { get; set; } = string.Empty;
    [Required]
    public string ColorCode { get; set; } = string.Empty;
    public decimal Probability { get; set; }
}

public class UpdateSegmentDto : CreateSegmentDto
{
    public bool IsActive { get; set; }
}
