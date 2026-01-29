using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs.Wheel;
using Nafes.API.Modules;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WheelController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WheelController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Save wheel game result and update student stats
    /// </summary>
    [AllowAnonymous] // Needed for students to save results
    [HttpPost("result")]
    public async Task<ActionResult<WheelGameResultResponse>> SaveResult([FromBody] WheelGameResultDto dto)
    {
        var student = await _context.Students.FindAsync(dto.StudentId);
        if (student == null)
        {
            return NotFound("Student not found");
        }

        // Create game result
        var result = new WheelGameResult
        {
            StudentId = dto.StudentId,
            FinalScore = dto.FinalScore,
            QuestionsAnswered = dto.QuestionsAnswered,
            CorrectAnswers = dto.CorrectAnswers,
            TimeSpentSeconds = dto.TimeSpentSeconds,
            PlayedAt = DateTime.UtcNow
        };

        _context.WheelGameResults.Add(result);

        // Update student stats
        student.WheelGamesPlayed++;
        student.TotalPoints += dto.FinalScore > 0 ? dto.FinalScore : 0; // Only add positive scores
        student.LastActiveAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Check for new achievements
        var newAchievements = await CheckWheelAchievements(student, dto);

        return Ok(new WheelGameResultResponse
        {
            Id = result.Id,
            FinalScore = dto.FinalScore,
            TotalPoints = student.TotalPoints,
            NewAchievements = newAchievements
        });
    }

    /// <summary>
    /// Get wheel game leaderboard
    /// </summary>
    [AllowAnonymous] // Needed for students to view leaderboard
    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<WheelLeaderboardEntry>>> GetLeaderboard()
    {
        var leaderboard = await _context.Students
            .Where(s => s.WheelGamesPlayed > 0)
            .OrderByDescending(s => s.TotalPoints)
            .ThenByDescending(s => s.WheelGamesPlayed)
            .Take(20)
            .Select(s => new WheelLeaderboardEntry
            {
                StudentId = s.Id,
                StudentName = s.Name,
                Grade = s.Grade,
                HighScore = s.WheelGameResults.Any() 
                    ? s.WheelGameResults.Max(r => r.FinalScore) 
                    : 0,
                TotalGames = s.WheelGamesPlayed,
                TotalPoints = s.TotalPoints
            })
            .ToListAsync();

        return Ok(leaderboard);
    }

    /// <summary>
    /// Get student's wheel game statistics
    /// </summary>
    [AllowAnonymous] // Needed for students to view their stats
    [HttpGet("stats/{studentId}")]
    public async Task<ActionResult> GetStudentStats(long studentId)
    {
        var student = await _context.Students
            .Include(s => s.WheelGameResults)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
        {
            return NotFound("Student not found");
        }

        var stats = new
        {
            TotalGames = student.WheelGamesPlayed,
            TotalPoints = student.TotalPoints,
            AverageScore = student.WheelGameResults.Any() 
                ? Math.Round(student.WheelGameResults.Average(r => r.FinalScore), 1) 
                : 0,
            HighScore = student.WheelGameResults.Any() 
                ? student.WheelGameResults.Max(r => r.FinalScore) 
                : 0,
            TotalQuestionsAnswered = student.WheelGameResults.Sum(r => r.QuestionsAnswered),
            TotalCorrectAnswers = student.WheelGameResults.Sum(r => r.CorrectAnswers),
            RecentGames = student.WheelGameResults
                .OrderByDescending(r => r.PlayedAt)
                .Take(5)
                .Select(r => new
                {
                    r.FinalScore,
                    r.QuestionsAnswered,
                    r.CorrectAnswers,
                    r.TimeSpentSeconds,
                    r.PlayedAt
                })
        };

        return Ok(stats);
    }

    /// <summary>
    /// Check and award wheel game achievements
    /// </summary>
    private async Task<List<string>> CheckWheelAchievements(Student student, WheelGameResultDto dto)
    {
        var newAchievements = new List<string>();
        var existingAchievements = await _context.StudentAchievements
            .Where(sa => sa.StudentId == student.Id)
            .Select(sa => sa.AchievementId)
            .ToListAsync();

        var allAchievements = await _context.Achievements.ToListAsync();

        // Check wheel-specific achievements
        foreach (var achievement in allAchievements)
        {
            if (existingAchievements.Contains(achievement.Id)) continue;

            bool earned = false;

            switch (achievement.Title)
            {
                case "دوار المعرفة":
                    earned = student.WheelGamesPlayed >= 5;
                    break;
                case "المحترف":
                    earned = dto.FinalScore >= 50;
                    break;
                case "البرق":
                    earned = dto.QuestionsAnswered >= 20;
                    break;
                case "الدقة المتناهية":
                    earned = dto.QuestionsAnswered >= 10 && dto.CorrectAnswers == dto.QuestionsAnswered;
                    break;
            }

            if (earned)
            {
                _context.StudentAchievements.Add(new StudentAchievement
                {
                    StudentId = student.Id,
                    AchievementId = achievement.Id,
                    DateUnlocked = DateTime.UtcNow
                });
                newAchievements.Add(achievement.Title);
            }
        }

        if (newAchievements.Any())
        {
            await _context.SaveChangesAsync();
        }

        return newAchievements;
    }
}
