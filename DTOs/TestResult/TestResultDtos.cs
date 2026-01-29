namespace Nafes.API.DTOs.TestResult;

public class TestResultGetDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public long GameId { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime DateTaken { get; set; }
    public int TimeSpent { get; set; }
    public bool Passed { get; set; }
    public List<DTOs.Achievement.AchievementDto> NewAchievements { get; set; } = new();
}

public class TestResultDetailDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public long GameId { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime DateTaken { get; set; }
    public int TimeSpent { get; set; }
    public bool Passed { get; set; }
    public string Answers { get; set; } = string.Empty;
}

public class TestResultCreateDto
{
    public long StudentId { get; set; }
    public long GameId { get; set; }
    public int Score { get; set; }
    public int TimeSpent { get; set; }
    public string Answers { get; set; } = string.Empty;
}

public class SubmitTestDto
{
    public long StudentId { get; set; }
    public long GameId { get; set; }
    public List<TestAnswerDto> Answers { get; set; } = new();
    public int TimeSpent { get; set; }
}

public class TestAnswerDto
{
    public long QuestionId { get; set; }
    public string Answer { get; set; } = string.Empty;
}
