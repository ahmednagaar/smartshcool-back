namespace Nafes.API.Modules;

public class StudentAchievement : BaseModel
{
    public long StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public long AchievementId { get; set; }
    public Achievement Achievement { get; set; } = null!;
    
    public DateTime DateUnlocked { get; set; } = DateTime.UtcNow;
}
