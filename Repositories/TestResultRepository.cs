using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public class TestResultRepository : GenericRepository<TestResult>, ITestResultRepository
{
    public TestResultRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TestResult>> GetByStudentIdAsync(long studentId)
    {
        return await _dbSet
            .Include(tr => tr.Game)
            .Where(tr => tr.StudentId == studentId && !tr.IsDeleted)
            .OrderByDescending(tr => tr.DateTaken)
            .ToListAsync();
    }

    public async Task<IEnumerable<TestResult>> GetByGameIdAsync(long gameId)
    {
        return await _dbSet
            .Include(tr => tr.Student)
            .Where(tr => tr.GameId == gameId && !tr.IsDeleted)
            .OrderByDescending(tr => tr.DateTaken)
            .ToListAsync();
    }

    public async Task<TestResult?> GetDetailedResultAsync(long id)
    {
        return await _dbSet
            .Include(tr => tr.Student)
            .Include(tr => tr.Game)
                .ThenInclude(g => g.GameQuestions)
                    .ThenInclude(gq => gq.Question)
            .FirstOrDefaultAsync(tr => tr.Id == id && !tr.IsDeleted);
    }

    public async Task<IEnumerable<DTOs.TestResult.LeaderboardEntryDto>> GetLeaderboardAsync(int topN)
    {
        var groupedResults = await _dbSet
            .Include(tr => tr.Student)
            .Where(tr => !tr.IsDeleted)
            .GroupBy(tr => tr.StudentId)
            .Select(g => new 
            {
                StudentId = g.Key,
                StudentName = g.First().Student.Name,
                Grade = g.First().Student.Grade,
                TotalScore = g.Sum(tr => tr.Score),
                TestsCompleted = g.Count()
            })
            .OrderByDescending(x => x.TotalScore)
            .Take(topN)
            .ToListAsync();

        return groupedResults.Select((x, index) => new DTOs.TestResult.LeaderboardEntryDto
        {
            Rank = index + 1,
            StudentName = x.StudentName,
            Grade = x.Grade,
            TotalScore = x.TotalScore,
            TestsCompleted = x.TestsCompleted,
            Badges = new List<string>() // Placeholder for achievements
        });
    }

    public async Task<DTOs.TestResult.StudentStatsDto> GetStudentStatsAsync(long studentId)
    {
        var results = await _dbSet
            .Include(tr => tr.Game)
            .Where(tr => tr.StudentId == studentId && !tr.IsDeleted)
            .ToListAsync();

        if (!results.Any())
        {
            return new DTOs.TestResult.StudentStatsDto();
        }

        var totalTests = results.Count;
        var totalTime = results.Sum(tr => tr.TimeSpent);
        var averageScore = results.Average(tr => tr.Score);

        // Mock subjects based on Game Title or a new field. Using simple keywords for now.
        // In a real app, Game should have a SubjectId or string Subject property.
        // We'll infer it from title for this enhancement.
        var subjectStats = results
            .GroupBy(tr => IdentifySubject(tr.Game?.Title ?? ""))
            .Select(g => new DTOs.TestResult.SubjectPerformanceDto
            {
                Subject = g.Key,
                Score = Math.Round(g.Average(tr => tr.Score), 1)
            })
            .ToList();

        // Weekly activity (last 7 days)
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.UtcNow.Date.AddDays(-i))
            .Reverse()
            .ToList();

        var weeklyActivity = last7Days.Select(date => new DTOs.TestResult.DailyActivityDto
        {
            Day = date.ToString("dddd", new System.Globalization.CultureInfo("ar-SA")),
            TestsCount = results.Count(tr => tr.DateTaken.Date == date)
        }).ToList();

        return new DTOs.TestResult.StudentStatsDto
        {
            TotalTests = totalTests,
            AverageScore = Math.Round(averageScore, 1),
            TotalTimeSpentMinutes = totalTime,
            CurrentLevel = (totalTests / 5) + 1, // Simple level logic
            SubjectPerformance = subjectStats,
            WeeklyActivity = weeklyActivity
        };
    }

    private string IdentifySubject(string title)
    {
        if (title.Contains("رياضيات") || title.Contains("Math")) return "الرياضيات";
        if (title.Contains("علوم") || title.Contains("Science")) return "العلوم";
        if (title.Contains("لغة") || title.Contains("Arabic")) return "اللغة العربية";
        return "عام";
    }

    public async Task<IEnumerable<DTOs.Analytics.ActivityDatasetDto>> GetActivityTrendsAsync(DateTime startDate, DateTime endDate)
    {
        var data = await _dbSet
            .Where(tr => !tr.IsDeleted && tr.DateTaken >= startDate && tr.DateTaken <= endDate)
            .GroupBy(tr => tr.DateTaken.Date)
            .Select(g => new {
                Date = g.Key,
                GamesPlayed = g.Count(),
                UniqueStudents = g.Select(tr => tr.StudentId).Distinct().Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var gamesPlayedData = new List<int>();
        var uniqueStudentsData = new List<int>();
        var labels = new List<string>();

        // Fill gaps with 0
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var entry = data.FirstOrDefault(d => d.Date == date);
            gamesPlayedData.Add(entry?.GamesPlayed ?? 0);
            uniqueStudentsData.Add(entry?.UniqueStudents ?? 0);
            // We return datasets, caller handles labels usually, but here we return datasets
            // Ideally we need labels too. The DTO defines activity dataset.
            // Let's assume the controller handles labels or we return a wrapper.
            // The interface returns IEnumerable<Dataset>, but the requirement was
            // Response: { labels: [...], datasets: [...] }
            // So we should return the wrapper DTO or handle it in controller.
            // Let's stick to returning datasets here and let controller build labels.
        }

        return new List<DTOs.Analytics.ActivityDatasetDto>
        {
            new DTOs.Analytics.ActivityDatasetDto { Label = "Games Played", Data = gamesPlayedData },
            new DTOs.Analytics.ActivityDatasetDto { Label = "Unique Students", Data = uniqueStudentsData }
        };
    }

    public async Task<IEnumerable<DTOs.Analytics.DifficultQuestionDto>> GetDifficultQuestionsAsync(int grade, int subject, int limit)
    {
        // MOCK IMPLEMENTATION: Real per-question stats require parsing JSON answers
        // Return random questions with mock error rates for UI demo
        var questionsQuery = _context.Set<Question>().AsQueryable();

        if (grade > 0) questionsQuery = questionsQuery.Where(q => (int)q.Grade == grade);
        if (subject > 0) questionsQuery = questionsQuery.Where(q => (int)q.Subject == subject);

        var questions = await questionsQuery
            .Take(limit * 2) // Take more to shuffle
            .ToListAsync();

        var random = new Random();
        
        return questions
            .OrderBy(x => random.Next())
            .Take(limit)
            .Select(q => new DTOs.Analytics.DifficultQuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                ErrorRate = Math.Round(random.NextDouble() * 100, 1),
                Attempts = random.Next(10, 500),
                IncorrectCount = 0 // Ignored for now
            })
            .OrderByDescending(q => q.ErrorRate)
            .ToList();
    }

    public async Task<DTOs.Analytics.EngagementSummaryDto> GetEngagementSummaryAsync()
    {
        var today = DateTime.UtcNow.Date;
        var startOfDay = today;
        var endOfDay = today.AddDays(1).AddTicks(-1);

        var dailyActiveUsers = await _dbSet
            .Where(tr => !tr.IsDeleted && tr.DateTaken >= startOfDay && tr.DateTaken <= endOfDay)
            .Select(tr => tr.StudentId)
            .Distinct()
            .CountAsync();

        var gamesPlayedToday = await _dbSet
            .CountAsync(tr => !tr.IsDeleted && tr.DateTaken >= startOfDay && tr.DateTaken <= endOfDay);

        var totalStudents = await _context.Set<Student>().CountAsync(s => !s.IsDeleted);

        // Calculate Average Session Duration (approximate from TimeSpent)
        // Taking average of all tests today
        var avgTimeSeconds = 0.0;
        var todayTests = await _dbSet
             .Where(tr => !tr.IsDeleted && tr.DateTaken >= startOfDay && tr.DateTaken <= endOfDay)
             .Select(tr => tr.TimeSpent)
             .ToListAsync();
             
        if (todayTests.Any())
        {
            avgTimeSeconds = todayTests.Average() * 60; // TimeSpent is in minutes
        }
        
        var popularGame = await _dbSet
            .Where(tr => !tr.IsDeleted)
            .Include(tr => tr.Game)
            .GroupBy(tr => tr.Game.Title)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        TimeSpan avgTimeSpan = TimeSpan.FromSeconds(avgTimeSeconds);
        string avgSessionDuration = string.Format("{0:D2}:{1:D2}", avgTimeSpan.Minutes, avgTimeSpan.Seconds);

        return new DTOs.Analytics.EngagementSummaryDto
        {
            DailyActiveUsers = dailyActiveUsers,
            GamesPlayedToday = gamesPlayedToday,
            TotalStudents = totalStudents,
            PopularGameMode = popularGame ?? "N/A",
            AvgSessionDuration = avgSessionDuration
        };
    }
}

