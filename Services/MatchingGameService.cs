using Nafes.API.DTOs.Matching;
using Nafes.API.DTOs.Shared;
using Nafes.API.Modules;
using Nafes.API.Repositories;

namespace Nafes.API.Services;

public interface IMatchingGameService
{
    // Student Methods
    Task<MatchingGameStartResponseDto> StartGameAsync(StartMatchingGameDto dto);
    Task<MatchResultDto> ValidateMatchAsync(ValidateMatchDto dto);
    Task<HintResponseDto> GetHintAsync(long sessionId);
    Task<SessionCompleteDto> CompleteSessionAsync(long sessionId);
    Task<IEnumerable<MatchingLeaderboardDto>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int top = 10);
    Task<IEnumerable<SessionCompleteDto>> GetStudentHistoryAsync(long studentId, GradeLevel? grade, SubjectType? subject);

    // Admin Methods
    Task<PaginatedResult<MatchingGameDto>> GetGamesAsync(int page, int pageSize, GradeLevel? grade, SubjectType? subject);
    Task<MatchingGameDto> GetGameByIdAsync(long id);
    Task<MatchingGameDto> CreateGameAsync(CreateMatchingGameDto dto);
    Task<MatchingGameDto> UpdateGameAsync(long id, UpdateMatchingGameDto dto);
    Task<bool> DeleteGameAsync(long id);
}

public class MatchingGameService : IMatchingGameService
{
    private readonly IMatchingGameRepository _gameRepository;
    private readonly IMatchingGameSessionRepository _sessionRepository;

    public MatchingGameService(IMatchingGameRepository gameRepository, IMatchingGameSessionRepository sessionRepository)
    {
        _gameRepository = gameRepository;
        _sessionRepository = sessionRepository;
    }

    // Student Implementation
    public async Task<MatchingGameStartResponseDto> StartGameAsync(StartMatchingGameDto dto)
    {
        MatchingGame? game;

        if (dto.GameId.HasValue)
        {
            game = await _gameRepository.GetByIdAsync(dto.GameId.Value, includePairs: true);
        }
        else
        {
            game = await _gameRepository.GetRandomGameAsync(dto.GradeId, dto.SubjectId, dto.DifficultyLevel);
        }

        if (game == null || !game.IsActive)
        {
            throw new Exception("No matching game found.");
        }

        var session = new MatchingGameSession
        {
            StudentId = dto.StudentId ?? 1, // Default to guest ID
            MatchingGameId = game.Id,
            GradeId = game.GradeId,
            SubjectId = game.SubjectId,
            TotalPairs = game.NumberOfPairs,
            StartTime = DateTime.UtcNow,
            IsCompleted = false
        };

        session = await _sessionRepository.CreateAsync(session);

        // Prepare Questions and Answers
        var questions = game.Pairs.OrderBy(p => p.PairOrder).Select(p => new GameCardDto
        {
            Id = p.Id,
            Text = p.QuestionText,
            ImageUrl = p.QuestionImageUrl,
            AudioUrl = p.QuestionAudioUrl,
            Type = p.QuestionType
        }).ToList();

        var random = new Random();
        var answers = game.Pairs.Select(p => new GameCardDto
        {
            Id = p.Id,
            Text = p.AnswerText,
            ImageUrl = p.AnswerImageUrl,
            AudioUrl = p.AnswerAudioUrl,
            Type = p.AnswerType
        }).OrderBy(x => random.Next()).ToList();

        return new MatchingGameStartResponseDto
        {
            SessionId = session.Id,
            GameTitle = game.GameTitle,
            Instructions = game.Instructions,
            NumberOfPairs = game.NumberOfPairs,
            MatchingMode = game.MatchingMode,
            UITheme = game.UITheme,
            ShowConnectingLines = game.ShowConnectingLines,
            EnableAudio = game.EnableAudio,
            EnableHints = game.EnableHints,
            MaxHints = game.MaxHints,
            TimerMode = game.TimerMode,
            TimeLimitSeconds = game.TimeLimitSeconds,
            Questions = questions,
            Answers = answers
        };
    }

    public async Task<MatchResultDto> ValidateMatchAsync(ValidateMatchDto dto)
    {
        var session = await _sessionRepository.GetByIdAsync(dto.SessionId);
        if (session == null) throw new Exception("Session not found");
        if (session.IsCompleted) throw new Exception("Game already completed");

        var existing = await _sessionRepository.GetAttemptBySessionAndPairAsync(dto.SessionId, dto.QuestionId);
        if (existing != null && existing.IsCorrect)
        {
            return new MatchResultDto { Message = "Already matched!" };
        }

        var game = await _gameRepository.GetByIdAsync(session.MatchingGameId, includePairs: true);
        if (game == null) throw new Exception("Game not found");

        bool isCorrect = dto.QuestionId == dto.AnswerId;

        session.TotalMoves++;
        int points = 0;
        string? explanation = null;

        if (isCorrect)
        {
            session.MatchedPairs++;
            points = game.PointsPerMatch;
            session.TotalScore += points;

            var pair = game.Pairs.FirstOrDefault(p => p.Id == dto.QuestionId);
            explanation = pair?.Explanation;
        }
        else
        {
            session.WrongAttempts++;
            session.TotalScore = Math.Max(0, session.TotalScore - game.WrongMatchPenalty);
            points = -game.WrongMatchPenalty;
        }

        await _sessionRepository.AddAttemptAsync(new MatchingAttempt
        {
            SessionId = session.Id,
            PairId = dto.QuestionId, 
            IsCorrect = isCorrect,
            PointsEarned = points,
            TimeToMatchMs = dto.TimeToMatchMs,
            AttemptTime = DateTime.UtcNow
        });

        await _sessionRepository.UpdateAsync(session);

        bool isGameComplete = session.MatchedPairs >= session.TotalPairs;

        return new MatchResultDto
        {
            IsCorrect = isCorrect,
            PointsEarned = points,
            TotalScore = session.TotalScore,
            MatchedPairs = session.MatchedPairs,
            TotalPairs = session.TotalPairs,
            Explanation = explanation,
            Message = isCorrect ? "Correct!" : "Try again",
            IsGameComplete = isGameComplete
        };
    }

    public async Task<HintResponseDto> GetHintAsync(long sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, includeAttempts: true);
        if (session == null) throw new Exception("Session not found");

        var game = await _gameRepository.GetByIdAsync(session.MatchingGameId, includePairs: true);
        if (game == null) throw new Exception("Game not found");

        if (!game.EnableHints) throw new Exception("Hints disabled");
        if (session.HintsUsed >= game.MaxHints) throw new Exception("No hints remaining");

        var matchedIds = session.Attempts.Where(a => a.IsCorrect).Select(a => a.PairId).ToList();
        var availablePair = game.Pairs.FirstOrDefault(p => !matchedIds.Contains(p.Id));

        if (availablePair == null) throw new Exception("All matched!");

        session.HintsUsed++;
        await _sessionRepository.UpdateAsync(session);

        return new HintResponseDto
        {
            QuestionId = availablePair.Id,
            AnswerId = availablePair.Id,
            HintsRemaining = game.MaxHints - session.HintsUsed,
            Message = "Here is a hint!"
        };
    }

    public async Task<SessionCompleteDto> CompleteSessionAsync(long sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null) throw new Exception("Session not found");

        if (!session.IsCompleted)
        {
            session.EndTime = DateTime.UtcNow;
            session.TimeSpentSeconds = (int)(session.EndTime.Value - session.StartTime).TotalSeconds;
            session.IsCompleted = true;
            session.StarRating = CalculateStarRating(session);
            await _sessionRepository.UpdateAsync(session);
        }

        var leaderboard = await _sessionRepository.GetLeaderboardAsync(session.GradeId, session.SubjectId, 100);
        var rank = leaderboard.ToList().FindIndex(l => l.StudentId == session.StudentId) + 1;
        if (rank == 0) rank = leaderboard.Count() + 1;

        return new SessionCompleteDto
        {
            SessionId = session.Id,
            FinalScore = session.TotalScore,
            MatchedPairs = session.MatchedPairs,
            TotalPairs = session.TotalPairs,
            TotalMoves = session.TotalMoves,
            WrongAttempts = session.WrongAttempts,
            TimeSpent = session.TimeSpentSeconds,
            StarRating = session.StarRating,
            Rank = rank
        };
    }

    private int CalculateStarRating(MatchingGameSession session)
    {
        if (session.WrongAttempts == 0 && session.HintsUsed == 0) return 3;
        if (session.WrongAttempts <= 2) return 2;
        return 1;
    }

    public async Task<IEnumerable<MatchingLeaderboardDto>> GetLeaderboardAsync(GradeLevel grade, SubjectType subject, int top = 10)
    {
        return await _sessionRepository.GetLeaderboardAsync(grade, subject, top);
    }

    public async Task<IEnumerable<SessionCompleteDto>> GetStudentHistoryAsync(long studentId, GradeLevel? grade, SubjectType? subject)
    {
        var sessions = await _sessionRepository.GetStudentHistoryAsync(studentId, grade, subject);
        return sessions.Select(s => new SessionCompleteDto
        {
            SessionId = s.Id,
            FinalScore = s.TotalScore,
            MatchedPairs = s.MatchedPairs,
            TotalPairs = s.TotalPairs,
            TimeSpent = s.TimeSpentSeconds,
            StarRating = s.StarRating
        });
    }

    // Admin Implementation
    public async Task<PaginatedResult<MatchingGameDto>> GetGamesAsync(int page, int pageSize, GradeLevel? grade, SubjectType? subject)
    {
        var result = await _gameRepository.GetAvailableGamesAsync(page, pageSize, grade, subject);
        
        var dtos = result.Items.Select(MapToDto).ToList();

        return new PaginatedResult<MatchingGameDto>
        {
            Items = dtos,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<MatchingGameDto> GetGameByIdAsync(long id)
    {
        var game = await _gameRepository.GetByIdAsync(id, includePairs: true);
        if (game == null) throw new Exception("Game not found");
        return MapToDto(game);
    }

    public async Task<MatchingGameDto> CreateGameAsync(CreateMatchingGameDto dto)
    {
        if (dto.Pairs.Count < 4) throw new Exception("Minimum 4 pairs required.");
        
        var game = new MatchingGame
        {
            GameTitle = dto.GameTitle,
            Instructions = dto.Instructions,
            GradeId = dto.GradeId,
            SubjectId = dto.SubjectId,
            NumberOfPairs = dto.Pairs.Count,
            MatchingMode = dto.MatchingMode,
            UITheme = dto.UITheme,
            ShowConnectingLines = dto.ShowConnectingLines,
            EnableAudio = dto.EnableAudio,
            EnableHints = dto.EnableHints,
            MaxHints = dto.MaxHints,
            TimerMode = dto.TimerMode,
            TimeLimitSeconds = dto.TimeLimitSeconds,
            PointsPerMatch = dto.PointsPerMatch,
            WrongMatchPenalty = dto.WrongMatchPenalty,
            DifficultyLevel = dto.DifficultyLevel,
            Category = dto.Category,
            Pairs = dto.Pairs.Select((p, index) => new MatchingGamePair
            {
                QuestionText = p.QuestionText,
                QuestionImageUrl = p.QuestionImageUrl,
                QuestionAudioUrl = p.QuestionAudioUrl,
                QuestionType = p.QuestionType,
                AnswerText = p.AnswerText,
                AnswerImageUrl = p.AnswerImageUrl,
                AnswerAudioUrl = p.AnswerAudioUrl,
                AnswerType = p.AnswerType,
                Explanation = p.Explanation,
                PairOrder = index + 1
            }).ToList()
        };

        var created = await _gameRepository.CreateAsync(game);
        return MapToDto(created);
    }

    public async Task<MatchingGameDto> UpdateGameAsync(long id, UpdateMatchingGameDto dto)
    {
        var game = await _gameRepository.GetByIdAsync(id, includePairs: true);
        if (game == null) throw new Exception("Game not found");

        game.GameTitle = dto.GameTitle;
        game.Instructions = dto.Instructions;
        game.GradeId = dto.GradeId;
        game.SubjectId = dto.SubjectId;
        game.MatchingMode = dto.MatchingMode;
        game.UITheme = dto.UITheme;
        game.ShowConnectingLines = dto.ShowConnectingLines;
        game.EnableAudio = dto.EnableAudio;
        game.EnableHints = dto.EnableHints;
        game.MaxHints = dto.MaxHints;
        game.TimerMode = dto.TimerMode;
        game.TimeLimitSeconds = dto.TimeLimitSeconds;
        game.PointsPerMatch = dto.PointsPerMatch;
        game.WrongMatchPenalty = dto.WrongMatchPenalty;
        game.DifficultyLevel = dto.DifficultyLevel;
        game.Category = dto.Category;
        game.IsActive = dto.IsActive;
        game.DisplayOrder = dto.DisplayOrder;
        game.NumberOfPairs = dto.Pairs.Count;

        game.Pairs.Clear(); 
        
        foreach (var pDto in dto.Pairs)
        {
             game.Pairs.Add(new MatchingGamePair
            {
                QuestionText = pDto.QuestionText,
                QuestionImageUrl = pDto.QuestionImageUrl,
                QuestionAudioUrl = pDto.QuestionAudioUrl,
                QuestionType = pDto.QuestionType,
                AnswerText = pDto.AnswerText,
                AnswerImageUrl = pDto.AnswerImageUrl,
                AnswerAudioUrl = pDto.AnswerAudioUrl,
                AnswerType = pDto.AnswerType,
                Explanation = pDto.Explanation
            });
        }
        
        int order = 1;
        foreach(var p in game.Pairs) p.PairOrder = order++;

        await _gameRepository.UpdateAsync(game);
        return MapToDto(game);
    }

    public async Task<bool> DeleteGameAsync(long id)
    {
        return await _gameRepository.DeleteAsync(id);
    }

    private MatchingGameDto MapToDto(MatchingGame game)
    {
        return new MatchingGameDto
        {
            Id = game.Id,
            GameTitle = game.GameTitle,
            Instructions = game.Instructions,
            GradeId = game.GradeId,
            SubjectId = game.SubjectId,
            NumberOfPairs = game.NumberOfPairs,
            MatchingMode = game.MatchingMode,
            UITheme = game.UITheme,
            ShowConnectingLines = game.ShowConnectingLines,
            EnableAudio = game.EnableAudio,
            EnableHints = game.EnableHints,
            MaxHints = game.MaxHints,
            TimerMode = game.TimerMode,
            TimeLimitSeconds = game.TimeLimitSeconds,
            PointsPerMatch = game.PointsPerMatch,
            WrongMatchPenalty = game.WrongMatchPenalty,
            DifficultyLevel = game.DifficultyLevel,
            Category = game.Category,
            IsActive = game.IsActive,
            DisplayOrder = game.DisplayOrder,
            CreatedDate = game.CreatedDate,
            Pairs = game.Pairs.Select(p => new MatchingGamePairDto
            {
                Id = p.Id,
                QuestionText = p.QuestionText,
                QuestionImageUrl = p.QuestionImageUrl,
                QuestionAudioUrl = p.QuestionAudioUrl,
                QuestionType = p.QuestionType,
                AnswerText = p.AnswerText,
                AnswerImageUrl = p.AnswerImageUrl,
                AnswerAudioUrl = p.AnswerAudioUrl,
                AnswerType = p.AnswerType,
                Explanation = p.Explanation,
                PairOrder = p.PairOrder
            }).ToList()
        };
    }
}
