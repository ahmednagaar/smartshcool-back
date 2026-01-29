using System.ComponentModel.DataAnnotations;

namespace Nafes.API.Modules;

public class Achievement : BaseModel
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "🏆";
    public int Points { get; set; } = 10;
    
    // Criteria for unlocking (simplified)
    public string CriteriaType { get; set; } = string.Empty; // e.g. "TestCount", "Score", "SubjectCount"
    public int CriteriaValue { get; set; } // e.g. 5 tests, 100 score
    public string? CriteriaSubject { get; set; } // e.g. "Mathematics"

    public ICollection<StudentAchievement> StudentAchievements { get; set; } = new List<StudentAchievement>();
}
