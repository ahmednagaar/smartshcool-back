using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nafes.API.Modules;

public enum SegmentType
{
    Points = 0,
    Bonus = 1,
    LoseTurn = 2,
    ExtraLife = 3,
    DoublePoints = 4,
    Mystery = 5
}

public class WheelSpinSegment : BaseModel
{
    public SegmentType SegmentType { get; set; }

    public int SegmentValue { get; set; }

    [Required]
    [MaxLength(100)]
    public string DisplayText { get; set; } = string.Empty;

    [Required]
    [MaxLength(7)]
    public string ColorCode { get; set; } = "#000000";

    [Column(TypeName = "decimal(5, 4)")]
    public decimal Probability { get; set; }

    public bool IsActive { get; set; } = true;
}
