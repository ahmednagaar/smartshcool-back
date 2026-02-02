using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs.Matching;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public class MatchingGameSessionRepository : IMatchingGameSessionRepository
{
    private readonly ApplicationDbContext _context;

    public MatchingGameSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MatchingGameSession> CreateAsync(MatchingGameSession session)
    {
        _context.MatchingGameSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<MatchingGameSession> UpdateAsync(MatchingGameSession session)
    {
        _context.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<MatchingGameSession?> GetByIdAsync(long sessionId, bool includeAttempts = false)
    {
        var query = _context.MatchingGameSessions.AsQueryable();
        if (includeAttempts)
        {
            query = query.Include(s => s.Attempts);
        }
        return await query.FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public async Task<IEnumerable<MatchingGameSession>> GetStudentHistoryAsync(long studentId, GradeLevel? grade = null, SubjectType? subject = null)
    {
        var query = _context.MatchingGameSessions
            .Where(s => s.StudentId == studentId);

        if (grade.HasValue)
            query = query.Where(s => s.GradeId == grade.Value);

        if (subject.HasValue)
            query = query.Where(s => s.SubjectId == subject.Value);

        return await query
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchingLeaderboardDto>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int top = 10)
    {
       var topSessions = await _context.MatchingGameSessions
           .Include(s => s.Student)
           .Where(s => s.GradeId == grade && s.SubjectId == subject && s.IsCompleted)
           .OrderByDescending(s => s.TotalScore)
           .ThenBy(s => s.TimeSpentSeconds)
           .Take(top)
           .ToListAsync();

       return topSessions.Select((s, index) => new MatchingLeaderboardDto
       {
           Rank = index + 1,
           StudentId = s.StudentId,
           StudentName = s.Student?.Name ?? "Unknown",
           Score = s.TotalScore,
           TimeSpent = s.TimeSpentSeconds,
           DatePlayed = s.StartTime
       });
    }

    public async Task<MatchingAttempt> AddAttemptAsync(MatchingAttempt attempt)
    {
        _context.MatchingAttempts.Add(attempt);
        await _context.SaveChangesAsync();
        return attempt;
    }

    public async Task<MatchingAttempt?> GetAttemptBySessionAndPairAsync(long sessionId, long pairId)
    {
        return await _context.MatchingAttempts
            .FirstOrDefaultAsync(a => a.SessionId == sessionId && a.PairId == pairId && a.IsCorrect);
    }
}
