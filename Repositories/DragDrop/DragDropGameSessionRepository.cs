using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs.DragDrop;
using Nafes.API.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nafes.API.Repositories;

public class DragDropGameSessionRepository : IDragDropGameSessionRepository
{
    private readonly ApplicationDbContext _context;

    public DragDropGameSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DragDropGameSession> CreateSessionAsync(DragDropGameSession session)
    {
        _context.DragDropGameSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<DragDropGameSession> UpdateSessionAsync(DragDropGameSession session)
    {
        _context.DragDropGameSessions.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<DragDropGameSession?> GetSessionByIdAsync(int sessionId, bool includeAttempts = false)
    {
        var query = _context.DragDropGameSessions
            .Include(s => s.DragDropQuestion)
                .ThenInclude(q => q.Zones)
            .Include(s => s.DragDropQuestion)
                .ThenInclude(q => q.Items)
            .AsQueryable();

        if (includeAttempts)
        {
            query = query.Include(s => s.Attempts)
                         .ThenInclude(a => a.Item);
        }

        return await query.FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public async Task<DragDropGameSession?> GetActiveSessionAsync(int studentId)
    {
        // Assuming "active" means not completed and started recently (e.g., within 24 hours) or just not completed?
        // Prompt says "GetActiveSession", typically means isCompleted = false
        return await _context.DragDropGameSessions
            .Include(s => s.DragDropQuestion)
            .Where(s => s.StudentId == studentId && !s.IsCompleted)
            .OrderByDescending(s => s.StartTime)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CompleteSessionAsync(int sessionId)
    {
        var session = await _context.DragDropGameSessions.FindAsync(sessionId);
        if (session == null) return false;

        session.IsCompleted = true;
        session.EndTime = DateTime.UtcNow;
        if (session.StartTime != DateTime.MinValue)
        {
             session.TimeSpentSeconds = (int)(session.EndTime.Value - session.StartTime).TotalSeconds;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DragDropGameSession>> GetStudentHistoryAsync(int studentId, GradeLevel? grade = null, SubjectType? subject = null)
    {
        var query = _context.DragDropGameSessions
            .Include(s => s.DragDropQuestion)
            .Where(s => s.StudentId == studentId && s.IsCompleted);

        if (grade.HasValue)
            query = query.Where(s => s.Grade == grade.Value);

        if (subject.HasValue)
            query = query.Where(s => s.Subject == subject.Value);

        return await query.OrderByDescending(s => s.EndTime).ToListAsync();
    }

    public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int topN = 10)
    {
        // Get top scoring sessions
        var topSessions = await _context.DragDropGameSessions
            .Where(s => s.Grade == grade && s.Subject == subject && s.IsCompleted)
            .OrderByDescending(s => s.TotalScore)
            .ThenBy(s => s.TimeSpentSeconds) // Tie breaker (less time is better?) Actually usually less time is better, so maybe ThenBy(s => s.TimeSpentSeconds)
            // But wait, TimeSpentSeconds ascending is better.
            .OrderByDescending(s => s.TotalScore)
            .ThenBy(s => s.TimeSpentSeconds) 
            .Take(topN * 5) // Take more to deduplicate students in memory
            .Select(s => new 
            {
                s.StudentId,
                StudentName = _context.Students.Where(st => st.Id == s.StudentId).Select(st => st.Name).FirstOrDefault() ?? "Unknown",
                s.TotalScore,
                Accuracy = s.TotalItems > 0 ? (double)s.CorrectPlacements / s.TotalItems * 100 : 0,
                s.EndTime,
                s.Grade,
                s.TimeSpentSeconds
            })
            .ToListAsync();

        // Distinct by StudentId in memory (Take the best score for each student)
        // Since we ordered by Score Desc, the first occurrence of StudentId is their best score.
        var distinctLeaderboard = topSessions
            .DistinctBy(s => s.StudentId)
            .Take(topN)
            .Select((s, index) => new LeaderboardEntry
            {
                Rank = index + 1,
                StudentId = s.StudentId,
                StudentName = s.StudentName,
                Score = s.TotalScore,
                Accuracy = s.Accuracy,
                DatePlayed = s.EndTime ?? DateTime.UtcNow,
                GradeName = s.Grade.ToString(),
                TimeSpentSeconds = s.TimeSpentSeconds
            })
            .ToList();

        return distinctLeaderboard;
    }
}
