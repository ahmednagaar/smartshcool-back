using System.Collections.Generic;
using System.Threading.Tasks;
using Nafes.API.DTOs; // Assuming
using Nafes.API.DTOs.TestResult;
using Nafes.API.DTOs.FlipCard;
using Nafes.API.Modules;

namespace Nafes.API.Repositories.FlipCard
{
    public interface IFlipCardGameSessionRepository
    {
        Task<FlipCardGameSession> CreateSessionAsync(FlipCardGameSession session);
        Task<FlipCardGameSession> UpdateSessionAsync(FlipCardGameSession session);
        Task<FlipCardGameSession?> GetSessionByIdAsync(int sessionId, bool includeAttempts = false);
        Task<FlipCardGameSession?> GetActiveSessionAsync(long studentId); // Using long for StudentId
        Task<bool> CompleteSessionAsync(int sessionId);
        Task<IEnumerable<FlipCardGameSession>> GetStudentHistoryAsync(long studentId, int? gradeId = null, int? subjectId = null);
        Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(int gradeId, int subjectId, int topN = 10);
        Task<StudentStatsDto> GetStudentStatisticsAsync(long studentId);
    }
}
