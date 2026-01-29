using Nafes.API.Modules;

namespace Nafes.API.DTOs.Game;

public class GameGetDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TimeLimit { get; set; }
    public int PassingScore { get; set; }

    public GradeLevel Grade { get; set; }
    public SubjectType Subject { get; set; }
    public int QuestionCount { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class GameWithQuestionsDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TimeLimit { get; set; }
    public int PassingScore { get; set; }
    public GradeLevel Grade { get; set; }
    public SubjectType Subject { get; set; }
    public List<GameQuestionDto> Questions { get; set; } = new();
}

public class GameQuestionDto
{
    public long QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public string? Options { get; set; }
    public int Order { get; set; }
}

public class GameCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TimeLimit { get; set; }
    public int PassingScore { get; set; }
    public GradeLevel Grade { get; set; }
    public SubjectType Subject { get; set; }
    public List<long> QuestionIds { get; set; } = new();
}

public class GameUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TimeLimit { get; set; }
    public int PassingScore { get; set; }
    public GradeLevel Grade { get; set; }
    public SubjectType Subject { get; set; }
    public List<long> QuestionIds { get; set; } = new();
}

public class AddQuestionsDto
{
    public List<long> QuestionIds { get; set; } = new();
}

public class ReorderQuestionsDto
{
    public List<QuestionOrderDto> Questions { get; set; } = new();
}

public class QuestionOrderDto
{
    public long QuestionId { get; set; }
    public int Order { get; set; }
}

