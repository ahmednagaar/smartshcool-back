using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nafes.API.DTOs.WheelGame;
using Nafes.API.Modules;
using Nafes.API.Repositories;
using System.Text.Json;

namespace Nafes.API.Services;

public interface IWheelGameService
{
    Task<StartWheelGameDto> StartGameAsync(StartWheelGameDto request); // Returns session info? Actually StartGameDto is request.
    Task<StartGameResponseDto> StartGameResponseAsync(StartWheelGameDto request);
    Task<SpinResultDto> SpinWheelAsync(SpinWheelDto request);
    Task<AnswerResultDto> SubmitAnswerAsync(SubmitAnswerDto request);
    Task<HintResponseDto> GetHintAsync(GetHintDto request);
    Task<SessionCompleteDto> CompleteSessionAsync(long sessionId);
    Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(int gradeId, int subjectId);
    Task<StudentStatisticsDto> GetStudentStatsAsync(long studentId);
    Task<bool> SeedMockDataAsync(); // Dev helper
}
// DTO helper for response
public class StartGameResponseDto 
{
    public long SessionId { get; set; }
    public int TotalQuestions { get; set; }
    public List<WheelQuestionResponseDto> Questions { get; set; } = new(); // Or just IDs if strict
    public WheelGameSession Session { get; set; } = null!;
}

public class SessionStateData
{
    public List<long> QuestionOrder { get; set; } = new();
    public int CurrentIndex { get; set; }
    public SpinResultDto? CurrentSpin { get; set; }
}

public class WheelGameService : IWheelGameService
{
    private readonly IWheelGameSessionRepository _sessionRepository;
    private readonly IWheelQuestionRepository _questionRepository;
    private readonly IWheelQuestionAttemptRepository _attemptRepository;
    private readonly IWheelSpinSegmentRepository _segmentRepository; // Assuming this exists or using Generic
    private readonly IMapper _mapper;

    public WheelGameService(
        IWheelGameSessionRepository sessionRepository,
        IWheelQuestionRepository questionRepository,
        IWheelQuestionAttemptRepository attemptRepository,
        IWheelSpinSegmentRepository segmentRepository,
        IMapper mapper)
    {
        _sessionRepository = sessionRepository;
        _questionRepository = questionRepository;
        _attemptRepository = attemptRepository;
        _segmentRepository = segmentRepository;
        _mapper = mapper;
    }

    public async Task<StartGameResponseDto> StartGameResponseAsync(StartWheelGameDto request)
    {
        // Check for active session first? Or just create new.
        // For simplicity, create new.
        
        int count = request.NumberOfQuestions ?? 10;
        var questions = await _questionRepository.GetRandomQuestionsAsync(
            (GradeLevel)request.GradeId, 
            (SubjectType)request.SubjectId, 
            request.TestType ?? TestType.Nafes,
            count, 
            request.DifficultyLevel);

        if (!questions.Any())
        {
            throw new InvalidOperationException("No questions available for this selection.");
        }

        var session = new WheelGameSession
        {
            StudentId = request.StudentId,
            GradeId = (GradeLevel)request.GradeId,
            SubjectId = (SubjectType)request.SubjectId,
            TotalQuestions = questions.Count(),
            StartTime = DateTime.UtcNow,
            SessionData = JsonSerializer.Serialize(new SessionStateData
            {
                QuestionOrder = questions.Select(q => q.Id).ToList(),
                CurrentIndex = 0
            })
        };

        await _sessionRepository.AddAsync(session);
        await _sessionRepository.SaveChangesAsync();

        return new StartGameResponseDto
        {
            SessionId = session.Id,
            TotalQuestions = session.TotalQuestions,
            Questions = _mapper.Map<List<WheelQuestionResponseDto>>(questions),
            Session = session
        };
    }

    public Task<StartWheelGameDto> StartGameAsync(StartWheelGameDto request) 
    {
        // Helper to match interface if needed, but sticking to ResponseDto
        throw new NotImplementedException(); 
    }

    public async Task<SpinResultDto> SpinWheelAsync(SpinWheelDto request)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId);
        if (session == null || session.IsCompleted) throw new KeyNotFoundException("Active session not found");

        var segments = await _segmentRepository.GetActiveSegmentsAsync();
        if (!segments.Any()) throw new InvalidOperationException("No wheel segments configured");

        // Weighted Random
        var totalWeight = segments.Sum(s => s.Probability);
        var randomValue = (decimal)Random.Shared.NextDouble() * totalWeight;
        
        WheelSpinSegment selected = segments.First();
        decimal currentSum = 0;
        foreach (var s in segments)
        {
            currentSum += s.Probability;
            if (randomValue <= currentSum)
            {
                selected = s;
                break;
            }
        }

        var result = new SpinResultDto
        {
            SegmentType = selected.SegmentType,
            Value = selected.SegmentValue,
            DisplayText = selected.DisplayText,
            ColorCode = selected.ColorCode,
            RotationDegrees = Random.Shared.Next(720, 1440) // Animation logic
        };

        // Update Session State
        var state = JsonSerializer.Deserialize<SessionStateData>(session.SessionData) ?? new SessionStateData();
        state.CurrentSpin = result;
        session.SessionData = JsonSerializer.Serialize(state);
        _sessionRepository.Update(session);
        await _sessionRepository.SaveChangesAsync();

        return result;
    }

    public async Task<AnswerResultDto> SubmitAnswerAsync(SubmitAnswerDto request)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId);
        if (session == null || session.IsCompleted) throw new KeyNotFoundException("Session not found");

        var question = await _questionRepository.GetByIdAsync(request.QuestionId);
        if (question == null) throw new KeyNotFoundException("Question not found");

        var state = JsonSerializer.Deserialize<SessionStateData>(session.SessionData) ?? new SessionStateData();
        
        // Validation: Ensure question matches current index?
        // if (state.QuestionOrder[state.CurrentIndex] != request.QuestionId) ... skip for now to be flexible

        bool isCorrect = CheckAnswer(request.StudentAnswer, question);
        int points = 0;

        if (isCorrect)
        {
            int basePoints = question.PointsValue;
            // Apply Spin Bonus
            if (state.CurrentSpin != null)
            {
                 // Logic: Add segment value or Apply multiplier
                 // If SegmentType == DoublePoints -> base * 2
                 // If SegmentType == Points -> base + Value
                 // If SegmentType == Bonus -> base + Value
                 
                 if (state.CurrentSpin.SegmentType == SegmentType.DoublePoints)
                     basePoints *= 2;
                 else if (state.CurrentSpin.SegmentType == SegmentType.Points || state.CurrentSpin.SegmentType == SegmentType.Bonus)
                     basePoints += state.CurrentSpin.Value;
            }

            // Hint Penalty (50%?)
            if (request.HintUsed)
                basePoints /= 2;

            points = basePoints;
        }

        // Record Attempt
        var attempt = new WheelQuestionAttempt
        {
            SessionId = session.Id,
            QuestionId = question.Id,
            StudentAnswer = request.StudentAnswer,
            IsCorrect = isCorrect,
            PointsEarned = points,
            TimeSpent = request.TimeSpent,
            HintUsed = request.HintUsed,
            SpinResult = state.CurrentSpin?.DisplayText ?? "",
            AttemptTime = DateTime.UtcNow
        };
        await _attemptRepository.AddAsync(attempt);
        // Note: SaveChanges will be called at end of method via session repo, sharing context.
        // But to be safe/explicit:
        // await _attemptRepository.SaveChangesAsync(); 
        // We will call SaveChanges on session update which commits all.

        // Update Session
        session.QuestionsAnswered++;
        session.TimeSpentSeconds += request.TimeSpent; // Add specific time
        if (isCorrect)
        {
            session.CorrectAnswers++;
            session.TotalScore += points;
        }
        else
        {
            session.WrongAnswers++;
        }

        // Check completion
        state.CurrentIndex++;
        bool isComplete = state.CurrentIndex >= state.QuestionOrder.Count;
        
        if (isComplete)
        {
            session.IsCompleted = true;
            session.EndTime = DateTime.UtcNow;
            
            // Re-calculate Total Time? Or trust accumulation
            // session.TimeSpentSeconds = (int)(session.EndTime.Value - session.StartTime).TotalSeconds;
        }
        
        state.CurrentSpin = null; // Reset spin
        session.SessionData = JsonSerializer.Serialize(state);
        _sessionRepository.Update(session);
        await _sessionRepository.SaveChangesAsync();

        return new AnswerResultDto
        {
            IsCorrect = isCorrect,
            PointsEarned = points,
            CorrectAnswer = question.CorrectAnswer,
            Explanation = question.Explanation,
            TotalScore = session.TotalScore,
            QuestionsRemaining = session.TotalQuestions - session.QuestionsAnswered,
            SessionComplete = isComplete,
            NextQuestionId = (!isComplete && state.CurrentIndex < state.QuestionOrder.Count) 
                ? state.QuestionOrder[state.CurrentIndex] : null
        };
    }

    private bool CheckAnswer(string studentAnswer, WheelQuestion question)
    {
        if (string.IsNullOrWhiteSpace(studentAnswer)) return false;
        
        // Simple normalization
        var s = studentAnswer.Trim().ToLowerInvariant();
        var c = question.CorrectAnswer.Trim().ToLowerInvariant();
        return s == c;
    }

    public async Task<HintResponseDto> GetHintAsync(GetHintDto request)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId);
        if (session == null) throw new KeyNotFoundException("Session not found");

        var question = await _questionRepository.GetByIdAsync(request.QuestionId);
        if (question == null) throw new KeyNotFoundException("Question not found");
        
        // Mark hint usage? Client sends HintUsed=true in Submit, but we track count here too?
        session.HintsUsed++;
        _sessionRepository.Update(session);
        await _sessionRepository.SaveChangesAsync();

        return new HintResponseDto
        {
            HintText = question.HintText ?? "No hint available.",
            PointsPenalty = 50 // e.g. 50%
        };
    }

    public async Task<SessionCompleteDto> CompleteSessionAsync(long sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null) throw new KeyNotFoundException("Session not found");

        if (!session.IsCompleted)
        {
            session.IsCompleted = true;
            session.EndTime = DateTime.UtcNow;
            _sessionRepository.Update(session);
            await _sessionRepository.SaveChangesAsync();
        }

        return new SessionCompleteDto
        {
            SessionId = session.Id,
            FinalScore = session.TotalScore,
            CorrectAnswers = session.CorrectAnswers,
            TotalQuestions = session.TotalQuestions,
            Accuracy = session.TotalQuestions > 0 ? (double)session.CorrectAnswers / session.TotalQuestions * 100 : 0,
            TimeSpent = session.TimeSpentSeconds,
            Rank = 0, // Calculate rank if needed
            Achievements = new List<string>(), // Calc achievements
            ImprovementTips = new List<string>()
        };
    }

    public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(int gradeId, int subjectId)
    {
        return await _sessionRepository.GetLeaderboardAsync((GradeLevel)gradeId, (SubjectType)subjectId);
    }

    public async Task<StudentStatisticsDto> GetStudentStatsAsync(long studentId)
    {
        return await _sessionRepository.GetStudentStatisticsAsync(studentId);
    }

    public async Task<bool> SeedMockDataAsync()
    {
        var count = await _questionRepository.GetQuestionCountByGradeSubjectAsync(GradeLevel.Grade4, SubjectType.Arabic);
        if (count > 0) return false;

        var questions = new List<WheelQuestion>
        {
            // Grade 4 Arabic
            new WheelQuestion { QuestionText = "ما عاصمة السعودية؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "الرياض", WrongAnswers = "[\"جدة\", \"مكة\", \"الدمام\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Arabic, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy, CategoryTag = "جغرافيا", Explanation = "الرياض هي العاصمة.", IsActive = true, CreatedDate = DateTime.UtcNow },
            new WheelQuestion { QuestionText = "ما ضد كلمة شجاع؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "جبان", WrongAnswers = "[\"قوي\", \"سريع\", \"ذكي\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Arabic, TestType = TestType.Central, DifficultyLevel = DifficultyLevel.Easy, IsActive = true, CreatedDate = DateTime.UtcNow },
            
            // Grade 4 Science
            new WheelQuestion { QuestionText = "حيوان يسمى سفينة الصحراء؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "الجمل", WrongAnswers = "[\"الحصان\", \"الفيل\", \"الأسد\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Science, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy, IsActive = true, CreatedDate = DateTime.UtcNow },
            new WheelQuestion { QuestionText = "ما هي الحالة الصلبة للماء؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "الثلج", WrongAnswers = "[\"البخار\", \"الماء\", \"الضباب\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Science, TestType = TestType.Central, DifficultyLevel = DifficultyLevel.Easy, IsActive = true, CreatedDate = DateTime.UtcNow },

            // Grade 4 Math
             new WheelQuestion { QuestionText = "5 * 5 = ?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "25", WrongAnswers = "[\"20\", \"30\", \"10\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Math, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy, IsActive = true, CreatedDate = DateTime.UtcNow },
             new WheelQuestion { QuestionText = "20 / 4 = ?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "5", WrongAnswers = "[\"4\", \"6\", \"8\"]", PointsValue = 10, GradeId = GradeLevel.Grade4, SubjectId = SubjectType.Math, TestType = TestType.Central, DifficultyLevel = DifficultyLevel.Easy, IsActive = true, CreatedDate = DateTime.UtcNow },

             // Grade 5 Arabic
             new WheelQuestion { QuestionText = "الفاعل يكون دائماً؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "مرفوعاً", WrongAnswers = "[\"منصوباً\", \"مجروراً\", \"ساكناً\"]", PointsValue = 15, GradeId = GradeLevel.Grade5, SubjectId = SubjectType.Arabic, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Medium, IsActive = true, CreatedDate = DateTime.UtcNow },
             
             // Grade 5 Science
             new WheelQuestion { QuestionText = "عدد كواكب المجموعة الشمسية؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "8", WrongAnswers = "[\"7\", \"9\", \"10\"]", PointsValue = 15, GradeId = GradeLevel.Grade5, SubjectId = SubjectType.Science, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Medium, IsActive = true, CreatedDate = DateTime.UtcNow },

             // Grade 5 Math
             new WheelQuestion { QuestionText = "محيط مربع ضلعه 5سم؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "20", WrongAnswers = "[\"25\", \"15\", \"10\"]", PointsValue = 15, GradeId = GradeLevel.Grade5, SubjectId = SubjectType.Math, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Medium, IsActive = true, CreatedDate = DateTime.UtcNow },

             // Grade 6
             new WheelQuestion { QuestionText = "إعراب المبتدأ؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "مرفوع", WrongAnswers = "[\"منصوب\", \"مجرور\"]", PointsValue = 15, GradeId = GradeLevel.Grade6, SubjectId = SubjectType.Arabic, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Medium, IsActive = true, CreatedDate = DateTime.UtcNow },
             new WheelQuestion { QuestionText = "3 أس 2 = ?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "9", WrongAnswers = "[\"6\", \"3\", \"12\"]", PointsValue = 20, GradeId = GradeLevel.Grade6, SubjectId = SubjectType.Math, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Hard, IsActive = true, CreatedDate = DateTime.UtcNow },
             new WheelQuestion { QuestionText = "الغاز الذي نتنفسه؟", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "الأكسجين", WrongAnswers = "[\"الهيدروجين\", \"النيتروجين\"]", PointsValue = 15, GradeId = GradeLevel.Grade6, SubjectId = SubjectType.Science, TestType = TestType.Nafes, DifficultyLevel = DifficultyLevel.Easy, IsActive = true, CreatedDate = DateTime.UtcNow }

        };

        await _questionRepository.BulkCreateAsync(questions);
        return true;
    }
}
