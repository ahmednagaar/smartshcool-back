using Nafes.API.DTOs.Matching;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IMatchingGameSessionRepository
{
    Task<MatchingGameSession> CreateAsync(MatchingGameSession session);
    Task<MatchingGameSession> UpdateAsync(MatchingGameSession session);
    Task<MatchingGameSession?> GetByIdAsync(long sessionId, bool includeAttempts = false);
    Task<IEnumerable<MatchingGameSession>> GetStudentHistoryAsync(long studentId, GradeLevel? grade = null, SubjectType? subject = null);
    Task<IEnumerable<MatchingLeaderboardDto>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int top = 10);
    Task<MatchingAttempt> AddAttemptAsync(MatchingAttempt attempt);
    Task<MatchingAttempt?> GetAttemptBySessionAndPairAsync(long sessionId, long pairId);
}
