namespace Nafes.API.Modules;

public class RefreshToken : BaseModel
{
    public string Token { get; set; } = string.Empty;
    public long AdminId { get; set; }
    public Admin Admin { get; set; } = null!;
    
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedDate { get; set; }
}
