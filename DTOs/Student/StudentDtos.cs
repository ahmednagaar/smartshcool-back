namespace Nafes.API.DTOs.Student;

public class StudentGetDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Grade { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class StudentCreateDto
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Grade { get; set; } = string.Empty;
}

public class StudentUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Grade { get; set; } = string.Empty;
}
