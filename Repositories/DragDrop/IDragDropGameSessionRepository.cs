using System.Collections.Generic;
using System.Threading.Tasks;
using Nafes.API.DTOs.DragDrop;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IDragDropGameSessionRepository
{
    Task<DragDropGameSession> CreateSessionAsync(DragDropGameSession session);
    Task<DragDropGameSession> UpdateSessionAsync(DragDropGameSession session);
    Task<DragDropGameSession?> GetSessionByIdAsync(int sessionId, bool includeAttempts = false);
    Task<DragDropGameSession?> GetActiveSessionAsync(int studentId);
    Task<bool> CompleteSessionAsync(int sessionId);
    Task<IEnumerable<DragDropGameSession>> GetStudentHistoryAsync(int studentId, GradeLevel? grade = null, SubjectType? subject = null);
    Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int topN = 10);
}
