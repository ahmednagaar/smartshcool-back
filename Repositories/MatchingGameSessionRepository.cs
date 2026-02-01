using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs.MatchingGame;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IMatchingGameSessionRepository : IGenericRepository<MatchingGameSession>
{
    Task<IEnumerable<MatchingGameSession>> GetStudentHistoryAsync(long studentId, GradeLevel? grade, SubjectType? subject);
    Task<IEnumerable<MatchingLeaderboardDto>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int top);
}

public class MatchingGameSessionRepository : GenericRepository<MatchingGameSession>, IMatchingGameSessionRepository
{
    public MatchingGameSessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MatchingGameSession>> GetStudentHistoryAsync(long studentId, GradeLevel? grade, SubjectType? subject)
    {
        var query = _dbSet.Where(s => s.StudentId == studentId && !s.IsDeleted);

        if (grade.HasValue)
            query = query.Where(s => s.GradeId == grade.Value);

        if (subject.HasValue)
            query = query.Where(s => s.SubjectId == subject.Value);

        return await query
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchingLeaderboardDto>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int top)
    {
        // For leaderboard, we typically want highest score.
        // If same score, least time spent wins.
        // If same time, most recent wins (or earliest, depending on rules - let's go with earliest achievement)
        
        var query = _dbSet
            .Where(s => s.GradeId == grade && s.SubjectId == subject && !s.IsDeleted)
            .Include(s => s.Student)
            .Select(s => new MatchingLeaderboardDto
            {
                StudentName = s.Student != null ? s.Student.Name : "Unknown",
                Score = s.Score,
                TimeSpent = s.TimeSpentSeconds,
                DatePlayed = s.StartTime
            })
            .OrderByDescending(dto => dto.Score)
            .ThenBy(dto => dto.TimeSpent)
            .Take(top);

        // Note: Rank calculation is often done in memory or via window functions.
        // For simplicity with EF Core simple querying, we'll fetch ordered list and assign rank in service/controller
        // OR we just return the ordered list.
        
        return await query.ToListAsync();
    }
}
