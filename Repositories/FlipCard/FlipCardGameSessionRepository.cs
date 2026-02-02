using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs;
using Nafes.API.DTOs.TestResult;
using Nafes.API.DTOs.FlipCard;
using Nafes.API.Modules;

namespace Nafes.API.Repositories.FlipCard
{
    public class FlipCardGameSessionRepository : IFlipCardGameSessionRepository
    {
        private readonly ApplicationDbContext _context;

        public FlipCardGameSessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FlipCardGameSession> CreateSessionAsync(FlipCardGameSession session)
        {
            _context.FlipCardGameSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<FlipCardGameSession> UpdateSessionAsync(FlipCardGameSession session)
        {
            _context.Entry(session).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<FlipCardGameSession?> GetSessionByIdAsync(int sessionId, bool includeAttempts = false)
        {
            var query = _context.FlipCardGameSessions
                .Include(s => s.FlipCardQuestion)
                .ThenInclude(q => q.Pairs)
                .AsQueryable();

            if (includeAttempts)
            {
                query = query.Include(s => s.Attempts);
            }

            return await query.FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        public async Task<FlipCardGameSession?> GetActiveSessionAsync(long studentId)
        {
            return await _context.FlipCardGameSessions
                .Include(s => s.FlipCardQuestion)
                .ThenInclude(q => q.Pairs)
                .Where(s => s.StudentId == studentId && !s.IsCompleted)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CompleteSessionAsync(int sessionId)
        {
            var session = await _context.FlipCardGameSessions.FindAsync(sessionId);
            if (session == null) return false;

            session.IsCompleted = true;
            session.EndTime = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<FlipCardGameSession>> GetStudentHistoryAsync(long studentId, int? gradeId = null, int? subjectId = null)
        {
            var query = _context.FlipCardGameSessions
                .Include(s => s.FlipCardQuestion)
                .Where(s => s.StudentId == studentId && s.IsCompleted);

            if (gradeId.HasValue)
                query = query.Where(s => (int)s.Grade == gradeId.Value);

            if (subjectId.HasValue)
                query = query.Where(s => (int)s.Subject == subjectId.Value);

            return await query
                .OrderByDescending(s => s.EndTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(int gradeId, int subjectId, int topN = 10)
        {
            // Simple leaderboard by Total Score
            return await _context.FlipCardGameSessions
                .Where(s => (int)s.Grade == gradeId && (int)s.Subject == subjectId && s.IsCompleted)
                .GroupBy(s => s.StudentId)
                .Select(g => new LeaderboardEntryDto
                {
                    StudentName = g.First().Student.Name, 
                    TotalScore = g.Sum(s => s.TotalScore),
                    TestsCompleted = g.Count(),
                    Grade = g.First().Student.Grade,
                    Rank = 0 // Needs to be calculated in memory or window function
                })
                .OrderByDescending(e => e.TotalScore)
                .Take(topN)
                .ToListAsync();
        }

        public async Task<StudentStatsDto> GetStudentStatisticsAsync(long studentId)
        {
            var sessions = await _context.FlipCardGameSessions
                .Where(s => s.StudentId == studentId && s.IsCompleted)
                .ToListAsync();

            if (!sessions.Any())
            {
                return new StudentStatsDto
                {
                    TotalTests = 0,
                    AverageScore = 0,
                    TotalTimeSpentMinutes = 0,
                    SubjectPerformance = new List<SubjectPerformanceDto>(),
                    WeeklyActivity = new List<DailyActivityDto>()
                };
            }

            return new StudentStatsDto
            {
                TotalTests = sessions.Count,
                AverageScore = (int)sessions.Average(s => s.TotalScore),
                TotalTimeSpentMinutes = sessions.Sum(s => s.TimeSpentSeconds) / 60,
                SubjectPerformance = sessions
                    .GroupBy(s => s.Subject)
                    .Select(g => new SubjectPerformanceDto
                    {
                        Subject = g.Key.ToString(),
                        Score = (int)g.Average(s => s.TotalScore)
                    }).ToList(),
                WeeklyActivity = new List<DailyActivityDto>() // Placeholder
            };
        }
    }
}
