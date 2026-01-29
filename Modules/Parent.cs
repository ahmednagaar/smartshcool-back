namespace Nafes.API.Modules;

public class Parent : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    // Navigation properties - A parent can have multiple children
    public ICollection<Student> Children { get; set; } = new List<Student>();
}
