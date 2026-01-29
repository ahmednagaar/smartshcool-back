using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs.TestResult;
using Nafes.API.Modules;
using System.Text.Json;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestResultController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TestResultController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestResultGetDto>>> GetAll()
    {
        var results = await _unitOfWork.TestResults.GetAllAsync();
        var resultDtos = _mapper.Map<IEnumerable<TestResultGetDto>>(results);
        return Ok(resultDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestResultDetailDto>> GetById(long id)
    {
        var result = await _unitOfWork.TestResults.GetDetailedResultAsync(id);
        if (result == null)
            return NotFound(new { message = "النتيجة غير موجودة" }); // Result not found

        var resultDto = _mapper.Map<TestResultDetailDto>(result);
        return Ok(resultDto);
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<TestResultGetDto>>> GetByStudent(long studentId)
    {
        var results = await _unitOfWork.TestResults.GetByStudentIdAsync(studentId);
        var resultDtos = _mapper.Map<IEnumerable<TestResultGetDto>>(results);
        return Ok(resultDtos);
    }

    [HttpGet("game/{gameId}")]
    public async Task<ActionResult<IEnumerable<TestResultGetDto>>> GetByGame(long gameId)
    {
        var results = await _unitOfWork.TestResults.GetByGameIdAsync(gameId);
        var resultDtos = _mapper.Map<IEnumerable<TestResultGetDto>>(results);
        return Ok(resultDtos);
    }

    [AllowAnonymous] // Needed for students to view leaderboard
    [HttpGet("leaderboard")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboard([FromQuery] int top = 10)
    {
        var leaderboard = await _unitOfWork.TestResults.GetLeaderboardAsync(top);
        return Ok(leaderboard);
    }

    [HttpGet("stats/{studentId}")]
    public async Task<ActionResult<StudentStatsDto>> GetStudentStats(long studentId)
    {
        var stats = await _unitOfWork.TestResults.GetStudentStatsAsync(studentId);
        return Ok(stats);
    }

    [AllowAnonymous] // Needed for students to submit test results
    [HttpPost("submit")]
    public async Task<ActionResult<TestResultGetDto>> SubmitTest([FromBody] SubmitTestDto submitDto)
    {
        // Verify student exists
        var student = await _unitOfWork.Students.GetByIdAsync(submitDto.StudentId);
        if (student == null)
            return NotFound(new { message = "الطالب غير موجود" });

        // Get game with questions
        var game = await _unitOfWork.Games.GetWithQuestionsAsync(submitDto.GameId);
        if (game == null)
            return NotFound(new { message = "اللعبة غير موجودة" });

        // Calculate score
        int correctAnswers = 0;
        int totalQuestions = game.GameQuestions.Count;

        foreach (var answer in submitDto.Answers)
        {
            var question = game.GameQuestions
                .FirstOrDefault(gq => gq.QuestionId == answer.QuestionId)?
                .Question;

            if (question != null && question.CorrectAnswer == answer.Answer)
            {
                correctAnswers++;
            }
        }

        int score = totalQuestions > 0 ? (correctAnswers * 100) / totalQuestions : 0;
        bool passed = score >= game.PassingScore;

        // Create test result
        var testResult = new TestResult
        {
            StudentId = submitDto.StudentId,
            GameId = submitDto.GameId,
            Score = score,
            TimeSpent = submitDto.TimeSpent,
            DateTaken = DateTime.UtcNow,
            Answers = JsonSerializer.Serialize(submitDto.Answers),
            Passed = passed
        };

        await _unitOfWork.TestResults.AddAsync(testResult);
        await _unitOfWork.CommitAsync();

        // Check for achievements
        var newAchievements = await CheckAchievementsAsync(student.Id, testResult, game);

        // Reload with navigation properties
        var savedResult = await _unitOfWork.TestResults.GetDetailedResultAsync(testResult.Id);
        var resultDto = _mapper.Map<TestResultGetDto>(savedResult);
        resultDto.NewAchievements = _mapper.Map<List<DTOs.Achievement.AchievementDto>>(newAchievements);

        return Ok(resultDto);
    }

    [HttpPost]
    public async Task<ActionResult<TestResultGetDto>> Create([FromBody] TestResultCreateDto createDto)
    {
        var testResult = _mapper.Map<TestResult>(createDto);
        
        // Determine if passed
        var game = await _unitOfWork.Games.GetByIdAsync(createDto.GameId);
        if (game != null)
        {
            testResult.Passed = testResult.Score >= game.PassingScore;
        }

        testResult.DateTaken = DateTime.UtcNow;
        
        await _unitOfWork.TestResults.AddAsync(testResult);
        await _unitOfWork.CommitAsync();

        var resultDto = _mapper.Map<TestResultGetDto>(testResult);
        return CreatedAtAction(nameof(GetById), new { id = testResult.Id }, resultDto);
    }
    
    private async Task<List<Achievement>> CheckAchievementsAsync(long studentId, TestResult currentResult, Game game)
    {
        var newUnlocked = new List<Achievement>();
        var _context = ((Repositories.GenericRepository<TestResult>)_unitOfWork.TestResults).Context; // Access context directly for this custom logic

        var allAchievements = await _context.Achievements.Where(a => !a.IsDeleted).ToListAsync();
        var studentAchievements = await _context.StudentAchievements
            .Where(sa => sa.StudentId == studentId && !sa.IsDeleted)
            .Select(sa => sa.AchievementId)
            .ToListAsync();

        var potentialAchievements = allAchievements.Where(a => !studentAchievements.Contains(a.Id)).ToList();
        
        // Get stats for criteria
        var studentResults = await _unitOfWork.TestResults.GetByStudentIdAsync(studentId);
        var testCount = studentResults.Count(); // Includes current one as we just saved it

        foreach (var achievement in potentialAchievements)
        {
            bool unlocked = false;

            switch (achievement.CriteriaType)
            {
                case "TestCount":
                    if (testCount >= achievement.CriteriaValue) unlocked = true;
                    break;
                case "Score":
                    if (currentResult.Score >= achievement.CriteriaValue) unlocked = true;
                    break;
                case "Time":
                    if (currentResult.TimeSpent <= achievement.CriteriaValue && currentResult.Score >= 60) unlocked = true; // Assume passing needed
                    break;
                case "SubjectCount":
                    if (!string.IsNullOrEmpty(achievement.CriteriaSubject))
                    {
                        var subjectCount = studentResults.Count(tr => tr.Game.Title.Contains(achievement.CriteriaSubject));
                        if (subjectCount >= achievement.CriteriaValue) unlocked = true;
                    }
                    break;
            }

            if (unlocked)
            {
                _context.StudentAchievements.Add(new StudentAchievement
                {
                    StudentId = studentId,
                    AchievementId = achievement.Id,
                    DateUnlocked = DateTime.UtcNow
                });
                newUnlocked.Add(achievement);
            }
        }

        if (newUnlocked.Any())
        {
            await _context.SaveChangesAsync();
        }

        return newUnlocked;
    }
}
