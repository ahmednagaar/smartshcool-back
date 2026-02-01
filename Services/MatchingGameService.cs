using Nafes.API.DTOs.MatchingGame;
using Nafes.API.Modules;
using Nafes.API.Repositories;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Nafes.API.Services;

public interface IMatchingGameService
{
    Task<GameStartResponseDto> StartGameAsync(StartGameDto dto);
    Task<GameResultDto> SubmitGameAsync(SubmitMatchingGameDto dto);
    Task<IEnumerable<MatchingLeaderboardDto>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int top = 10);
    Task<IEnumerable<GameResultDto>> GetStudentHistoryAsync(long studentId, GradeLevel? grade, SubjectType? subject);
}

public class MatchingGameService : IMatchingGameService
{
    private readonly IMatchingGameSessionRepository _sessionRepository;
    private readonly IMatchingQuestionRepository _questionRepository;

    public MatchingGameService(IMatchingGameSessionRepository sessionRepository, IMatchingQuestionRepository questionRepository)
    {
        _sessionRepository = sessionRepository;
        _questionRepository = questionRepository;
    }

    public async Task<GameStartResponseDto> StartGameAsync(StartGameDto dto)
    {
        // 1. Get questions for grade/subject
        var questions = await _questionRepository.GetByGradeAndSubjectAsync(dto.GradeId, dto.SubjectId);
        
        // Take random 5-10 questions if many exist, for now take all (or limit logic here)
        var selectedQuestions = questions.Take(10).ToList();

        if (!selectedQuestions.Any())
        {
            throw new Exception("No matching questions found for this grade and subject.");
        }

        // 2. Create Session
        var session = new MatchingGameSession
        {
            StudentId = dto.StudentId,
            GradeId = dto.GradeId,
            SubjectId = dto.SubjectId,
            StartTime = DateTime.UtcNow,
            TotalQuestions = selectedQuestions.Count
        };

        await _sessionRepository.AddAsync(session);
        await _sessionRepository.SaveChangesAsync();

        // 3. Prepare Game Data (Shuffle)
        var response = new GameStartResponseDto
        {
            SessionId = session.Id,
            LeftItems = new List<GameLeftItemDto>(),
            RightItems = new List<GameRightItemDto>()
        };

        // Populate Left Items
        foreach (var q in selectedQuestions)
        {
            response.LeftItems.Add(new GameLeftItemDto
            {
                Id = q.Id,
                Text = q.LeftItemText,
                RightItemId = $"{q.Id}_correct"
            });

            // Add correct answer to pool
            response.RightItems.Add(new GameRightItemDto
            {
                Id = $"{q.Id}_correct", // Format: {QuestionId}_correct
                Text = q.RightItemText
            });

            // Add distractors to pool
            var distractors = JsonSerializer.Deserialize<List<string>>(q.DistractorItems) ?? new List<string>();
            int dIndex = 0;
            foreach (var d in distractors)
            {
                response.RightItems.Add(new GameRightItemDto
                {
                    Id = $"{q.Id}_distractor_{dIndex++}",
                    Text = d
                });
            }
        }

        // Shuffle Right Items
        var rng = new Random();
        response.RightItems = response.RightItems.OrderBy(x => rng.Next()).ToList();
        
        // Shuffle Left Items too? Usually fixed or shuffled. Let's shuffle.
        response.LeftItems = response.LeftItems.OrderBy(x => rng.Next()).ToList();

        return response;
    }

    public async Task<GameResultDto> SubmitGameAsync(SubmitMatchingGameDto dto)
    {
        var session = await _sessionRepository.GetByIdAsync(dto.SessionId);
        if (session == null)
            throw new KeyNotFoundException("Session not found");

        session.EndTime = DateTime.UtcNow;
        session.TimeSpentSeconds = dto.TimeSpentSeconds;

        int correctCount = 0;
        int score = 0;
        var detailedResults = new List<MatchResultDetailDto>();

        foreach (var match in dto.Matches)
        {
            var question = await _questionRepository.GetByIdAsync(match.QuestionId);
            if (question == null) continue;

            // Check if RightItemId indicates correctness
            // Logic: RightItemId format is "{QuestionId}_correct"
            bool isCorrect = match.RightItemId == $"{match.QuestionId}_correct";

            if (isCorrect)
            {
                correctCount++;
                score += 10; // 10 points per correct match
            }

            detailedResults.Add(new MatchResultDetailDto
            {
                QuestionId = match.QuestionId,
                IsCorrect = isCorrect,
                CorrectAnswer = question.RightItemText
            });
        }

        // Bonus for speed? (Optional)
        // if (dto.TimeSpentSeconds < 60) score += 50;

        session.CorrectMatches = correctCount;
        session.Score = score;

        _sessionRepository.Update(session);
        await _sessionRepository.SaveChangesAsync();

        return new GameResultDto
        {
            SessionId = session.Id,
            Score = score,
            TotalQuestions = session.TotalQuestions,
            CorrectMatches = correctCount,
            TimeSpentSeconds = dto.TimeSpentSeconds,
            Details = detailedResults
        };
    }

    public async Task<IEnumerable<MatchingLeaderboardDto>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int top = 10)
    {
        var leaderboard = await _sessionRepository.GetLeaderboardAsync(grade, subject, top);
        
        // Assign ranks in memory since repo returned ordered list
        int rank = 1;
        foreach (var item in leaderboard)
        {
            item.Rank = rank++;
        }
        
        return leaderboard;
    }

    public async Task<IEnumerable<GameResultDto>> GetStudentHistoryAsync(long studentId, GradeLevel? grade, SubjectType? subject)
    {
        var sessions = await _sessionRepository.GetStudentHistoryAsync(studentId, grade, subject);
        
        return sessions.Select(s => new GameResultDto
        {
            SessionId = s.Id,
            Score = s.Score,
            TotalQuestions = s.TotalQuestions,
            CorrectMatches = s.CorrectMatches,
            TimeSpentSeconds = s.TimeSpentSeconds,
            Details = new List<MatchResultDetailDto>() // History overview doesn't need deep details usually
        });
    }
}
