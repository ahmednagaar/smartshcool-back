using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IQuestionRepository : IGenericRepository<Question>
{
    Task<IEnumerable<Question>> GetByDifficultyAsync(DifficultyLevel difficulty);
    Task<IEnumerable<Question>> GetByTypeAsync(QuestionType type);
    Task<IEnumerable<Question>> GetFilteredAsync(GradeLevel grade, SubjectType subject, TestType testType);
    Task<(IEnumerable<Question> Items, int TotalCount)> SearchAsync(DTOs.Question.QuestionSearchRequestDto searchDto);
    Task<IEnumerable<Question>> GetAvailableForGameAsync(long gameId, GradeLevel grade, SubjectType subject, string? searchTerm = null);
    Task<Question?> GetIncludeDeletedAsync(long id);
}

