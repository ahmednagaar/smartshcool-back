using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface ITestResultRepository : IGenericRepository<TestResult>
{
    Task<IEnumerable<TestResult>> GetByStudentIdAsync(long studentId);
    Task<IEnumerable<TestResult>> GetByGameIdAsync(long gameId);
    Task<TestResult?> GetDetailedResultAsync(long id);
    
    // New Feature Methods
    Task<IEnumerable<DTOs.TestResult.LeaderboardEntryDto>> GetLeaderboardAsync(int topN);
    Task<DTOs.TestResult.StudentStatsDto> GetStudentStatsAsync(long studentId);
    Task<IEnumerable<DTOs.Analytics.ActivityDatasetDto>> GetActivityTrendsAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<DTOs.Analytics.DifficultQuestionDto>> GetDifficultQuestionsAsync(int grade, int subject, int limit);
    Task<DTOs.Analytics.EngagementSummaryDto> GetEngagementSummaryAsync();
}
