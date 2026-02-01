using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs.WheelGame;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IWheelGameSessionRepository : IGenericRepository<WheelGameSession>
{
    Task<WheelGameSession?> GetActiveSessionAsync(long studentId);
    Task<WheelGameSession?> GetSessionWithDetailsAsync(long sessionId);
    Task<(IEnumerable<WheelGameSession> Items, int TotalCount)> GetStudentHistoryAsync(long studentId, int page, int pageSize, GradeLevel? grade = null, SubjectType? subject = null);
    Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int topN = 10);
    Task<StudentStatisticsDto> GetStudentStatisticsAsync(long studentId);
}

public class WheelGameSessionRepository : GenericRepository<WheelGameSession>, IWheelGameSessionRepository
{
    public WheelGameSessionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<WheelGameSession?> GetActiveSessionAsync(long studentId)
    {
        return await _dbSet
            .Include(s => s.Attempts)
            .Where(s => s.StudentId == studentId && !s.IsCompleted && !s.IsDeleted)
            .OrderByDescending(s => s.StartTime)
            .FirstOrDefaultAsync();
    }
    
    public async Task<WheelGameSession?> GetSessionWithDetailsAsync(long sessionId)
    {
        return await _dbSet
            .Include(s => s.Attempts)
            .ThenInclude(a => a.Question)
            .FirstOrDefaultAsync(s => s.Id == sessionId && !s.IsDeleted);
    }

    public async Task<(IEnumerable<WheelGameSession> Items, int TotalCount)> GetStudentHistoryAsync(long studentId, int page, int pageSize, GradeLevel? grade = null, SubjectType? subject = null)
    {
        var query = _dbSet.Where(s => s.StudentId == studentId && s.IsCompleted && !s.IsDeleted);

        if (grade.HasValue) query = query.Where(s => s.GradeId == grade.Value);
        if (subject.HasValue) query = query.Where(s => s.SubjectId == subject.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.EndTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int topN = 10)
    {
        return await _dbSet
            .Where(s => s.GradeId == grade && s.SubjectId == subject && s.IsCompleted && !s.IsDeleted)
            .OrderByDescending(s => s.TotalScore)
            .ThenByDescending(s => s.CorrectAnswers)
            .ThenBy(s => s.TimeSpentSeconds)
            .Take(topN)
            .Select(s => new LeaderboardEntryDto
            {
                StudentId = s.StudentId ?? 0,
                StudentName = s.Student != null ? s.Student.Name : "زائر",
                Score = s.TotalScore,
                Accuracy = s.TotalQuestions > 0 ? (double)s.CorrectAnswers / s.TotalQuestions * 100 : 0,
                TimeSpent = s.TimeSpentSeconds,
                DatePlayed = s.EndTime ?? s.StartTime
            })
            .ToListAsync();
    }

    public async Task<StudentStatisticsDto> GetStudentStatisticsAsync(long studentId)
    {
        var sessions = await _dbSet
            .Where(s => s.StudentId == studentId && s.IsCompleted && !s.IsDeleted)
            .ToListAsync();

        if (!sessions.Any())
            return new StudentStatisticsDto();

        return new StudentStatisticsDto
        {
            TotalGamesPlayed = sessions.Count,
            AverageScore = sessions.Average(s => s.TotalScore),
            BestScore = sessions.Max(s => s.TotalScore),
            TotalCorrectAnswers = sessions.Sum(s => s.CorrectAnswers),
            TotalQuestionsAnswered = sessions.Sum(s => s.QuestionsAnswered),
            AverageTimePerGame = sessions.Average(s => s.TimeSpentSeconds),
            HintsUsedTotal = sessions.Sum(s => s.HintsUsed),
            FavoriteSubject = sessions
                .GroupBy(s => s.SubjectId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key.ToString() ?? "None"
        };
    }
}
