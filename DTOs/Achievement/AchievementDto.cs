namespace Nafes.API.DTOs.Achievement;

public class AchievementDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Points { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? DateUnlocked { get; set; }
}
