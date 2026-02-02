using Nafes.API.DTOs.Shared;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IMatchingGameRepository
{
    Task<MatchingGame?> GetByIdAsync(long id, bool includePairs = false);
    Task<IEnumerable<MatchingGame>> GetAllAsync();
    Task<PaginatedResult<MatchingGame>> GetAvailableGamesAsync(int page, int pageSize, GradeLevel? grade = null, SubjectType? subject = null);
    Task<MatchingGame> CreateAsync(MatchingGame game);
    Task<MatchingGame> UpdateAsync(MatchingGame game);
    Task<bool> DeleteAsync(long id);
    Task<MatchingGame?> GetRandomGameAsync(GradeLevel grade, SubjectType subject, DifficultyLevel? difficulty = null);
}
