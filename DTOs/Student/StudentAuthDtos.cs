namespace Nafes.API.DTOs.Student;

public class StudentLoginDto
{
    public string StudentCode { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
}

public class StudentRegisterDto
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty; // 4-digit PIN
}

public class StudentLoginResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StudentCode { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}
