namespace Nafes.API.Modules;

public class Admin : BaseModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public AdminRole Role { get; set; } = AdminRole.Admin;
    
    public DateTime? LastLogin { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public bool IsLocked { get; set; } = false;
    public DateTime? LockedUntil { get; set; }
    public bool IsApproved { get; set; } = false; // Requires SuperAdmin approval
    
    // Navigation properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public enum AdminRole
{
    SuperAdmin,
    Admin,
    Editor
}
