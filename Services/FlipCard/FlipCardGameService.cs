using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nafes.API.DTOs.FlipCard;
using Nafes.API.DTOs.TestResult;
using Nafes.API.Modules;
using Nafes.API.Repositories.FlipCard;

namespace Nafes.API.Services.FlipCard
{
    public class FlipCardGameService : IFlipCardGameService
    {
        private readonly IFlipCardGameSessionRepository _sessionRepository;
        private readonly IFlipCardQuestionRepository _questionRepository;
        private readonly IFlipCardAttemptRepository _attemptRepository;

        public FlipCardGameService(
            IFlipCardGameSessionRepository sessionRepository,
            IFlipCardQuestionRepository questionRepository,
            IFlipCardAttemptRepository attemptRepository)
        {
            _sessionRepository = sessionRepository;
            _questionRepository = questionRepository;
            _attemptRepository = attemptRepository;
        }

        public async Task<FlipCardGameSession> StartSessionAsync(StartFlipCardGameDto dto)
        {
            // Determine if this is an anonymous (guest) player
            bool isAnonymous = dto.StudentId <= 0;

            // Check for active session (only for logged-in students)
            if (!isAnonymous)
            {
                var activeSession = await _sessionRepository.GetActiveSessionAsync(dto.StudentId);
                if (activeSession != null)
                {
                    return activeSession; 
                }
            }

            // Get question
            FlipCardQuestion? question;
            if (dto.QuestionId.HasValue)
            {
                question = await _questionRepository.GetByIdAsync(dto.QuestionId.Value, includePairs: true);
            }
            else
            {
                question = await _questionRepository.GetRandomQuestionAsync(dto.GradeId, dto.SubjectId, dto.DifficultyLevel);
            }

            if (question == null)
                throw new Exception("No flip card games available for this selection");

            // For anonymous users: return virtual session without DB persistence
            if (isAnonymous)
            {
                return new FlipCardGameSession
                {
                    Id = -1, // Virtual session ID
                    StudentId = 0,
                    FlipCardQuestionId = question.Id,
                    FlipCardQuestion = question,
                    Grade = (GradeLevel)dto.GradeId,
                    Subject = (SubjectType)dto.SubjectId,
                    GameMode = dto.GameMode ?? question.GameMode,
                    TotalPairs = question.NumberOfPairs,
                    StartTime = DateTime.UtcNow,
                    IsCompleted = false
                };
            }

            // For logged-in students: persist session to DB
            var session = new FlipCardGameSession
            {
                StudentId = dto.StudentId,
                FlipCardQuestionId = question.Id,
                Grade = (GradeLevel)dto.GradeId,
                Subject = (SubjectType)dto.SubjectId,
                GameMode = dto.GameMode ?? question.GameMode,
                TotalPairs = question.NumberOfPairs,
                StartTime = DateTime.UtcNow,
                IsCompleted = false
            };

            session = await _sessionRepository.CreateSessionAsync(session);

            // Attach question with pairs for the controller to use
            session.FlipCardQuestion = question;
            
            return session;
        }

        public async Task<MatchResultDto> RecordMatchAsync(RecordMatchDto dto)
        {
            var session = await _sessionRepository.GetSessionByIdAsync(dto.SessionId);
            if (session == null) throw new Exception("Session not found");

            // We need Pair info to get DifficultyWeight or Explanation
             // But session.FlipCardQuestion might not be fully loaded? GetSessionByIdAsync implementation includes it?
             // Yes, implementation includes FlipCardQuestion and Pairs.
            
            var pair = session.FlipCardQuestion.Pairs.FirstOrDefault(p => p.Id == dto.PairId);
            if (pair == null) throw new Exception("Pair not found");

            // Calculate points
            var timeForMatch = dto.Card2FlippedAtMs - dto.Card1FlippedAtMs; // Duration of this specific move? Or total game time?
            // "Card1FlippedAtMs" usually implies timestamp relative to game start.
            
            var points = CalculatePoints(
                session.FlipCardQuestion.PointsPerMatch,
                Math.Abs(timeForMatch), // ensure positive
                session.FlipCardQuestion.DifficultyLevel,
                dto.HintUsed
            );

            // Record attempt
            var attempt = new FlipCardAttempt
            {
                SessionId = dto.SessionId,
                PairId = dto.PairId,
                Card1FlippedAtMs = dto.Card1FlippedAtMs,
                Card2FlippedAtMs = dto.Card2FlippedAtMs,
                IsCorrectMatch = true,
                PointsEarned = points,
                AttemptsBeforeMatch = dto.AttemptsBeforeMatch,
                AttemptTime = DateTime.UtcNow
            };

            await _attemptRepository.CreateAsync(attempt);

            // Update session
            session.MatchedPairs++;
            session.TotalMoves += dto.AttemptsBeforeMatch; // Add moves from this sequence
            session.TotalScore += points;
            
            await _sessionRepository.UpdateSessionAsync(session);

            return new MatchResultDto
            {
                IsCorrect = true,
                PointsEarned = points,
                TotalScore = session.TotalScore,
                MatchedPairs = session.MatchedPairs,
                TotalPairs = session.TotalPairs,
                Explanation = pair.Explanation ?? string.Empty,
                IsGameComplete = session.MatchedPairs >= session.TotalPairs
            };
        }

        public async Task<object> RecordWrongMatchAsync(RecordWrongMatchDto dto)
        {
            var session = await _sessionRepository.GetSessionByIdAsync(dto.SessionId);
            if (session == null) throw new Exception("Session not found");

            session.WrongAttempts++;
            session.TotalMoves++;
            if (session.TotalScore > session.FlipCardQuestion.MovePenalty)
                session.TotalScore -= session.FlipCardQuestion.MovePenalty;
            else
                session.TotalScore = 0; // Don't go negative?

            await _sessionRepository.UpdateSessionAsync(session);

            return new
            {
                penaltyApplied = session.FlipCardQuestion.MovePenalty,
                totalScore = session.TotalScore,
                message = "Not a match! Try again."
            };
        }

        public async Task<object> GetHintAsync(GetHintDto dto)
        {
            var session = await _sessionRepository.GetSessionByIdAsync(dto.SessionId, includeAttempts: true);
            if (session == null) throw new Exception("Session not found");

            if (session.HintsUsed >= session.FlipCardQuestion.MaxHints)
            {
                 throw new Exception("No hints remaining");
            }

            // Find unmatched pairs
            var matchedPairIds = session.Attempts.Where(a => a.IsCorrectMatch).Select(a => a.PairId).ToList();
            var unmatchedPair = session.FlipCardQuestion.Pairs
                .FirstOrDefault(p => !matchedPairIds.Contains(p.Id));

            if (unmatchedPair == null) throw new Exception("All pairs already matched!");

            session.HintsUsed++;
            await _sessionRepository.UpdateSessionAsync(session);

            return new
            {
                hintPairId = unmatchedPair.Id,
                card1 = new
                {
                    unmatchedPair.Card1Text,
                    unmatchedPair.Card1ImageUrl
                },
                hintsRemaining = session.FlipCardQuestion.MaxHints - session.HintsUsed,
                message = "Hint revealed! Find its match."
            };
        }

        public async Task<SessionCompleteDto> CompleteSessionAsync(int sessionId)
        {
            var session = await _sessionRepository.GetSessionByIdAsync(sessionId, includeAttempts: true);
            if (session == null) throw new Exception("Session not found");

            session.EndTime = DateTime.UtcNow;
            session.TimeSpentSeconds = (int)(session.EndTime.Value - session.StartTime).TotalSeconds;
            session.IsCompleted = true;
            session.StarRating = CalculateStarRating(session);

            await _sessionRepository.UpdateSessionAsync(session); // Or CompleteSessionAsync method which does specific things
            // We can call generic Update or specific. The Repo has CompleteSessionAsync which sets IsCompleted/EndTime, 
            // but we calc StarRating etc here.
            // Let's use Update logic since we modified properties.
            // Or Repo's CompleteSessionAsync does it. Logic was duplicated in repo impl. 
            // I'll stick to updating existing fields here.
            
            // Re-fetch (or reuse) for Rank
            var leaderboard = await _sessionRepository.GetLeaderboardAsync((int)session.Grade, (int)session.Subject, 100);
            var studentName = session.Student?.Name ?? "زائر";
            var rank = leaderboard.ToList().FindIndex(e => e.StudentName == studentName) + 1;

            return new SessionCompleteDto
            {
                SessionId = sessionId,
                FinalScore = session.TotalScore,
                MatchedPairs = session.MatchedPairs,
                TotalPairs = session.TotalPairs,
                TotalMoves = session.TotalMoves,
                TimeSpent = session.TimeSpentSeconds,
                StarRating = session.StarRating,
                Rank = rank > 0 ? rank : 999,
                Attempts = session.Attempts.Select(a => new AttemptDetailDto
                {
                    PairId = a.PairId,
                    IsCorrect = a.IsCorrectMatch,
                    Points = a.PointsEarned,
                    TimeMs = a.Card2FlippedAtMs - a.Card1FlippedAtMs
                }).ToList(),
                Achievements = CheckAchievements(session)
            };
        }

        private int CalculatePoints(int basePoints, int timeForMatch, DifficultyLevel difficultyLevel, bool hintUsed)
        {
            var points = basePoints;

            // Time bonus
            if (timeForMatch < 2000) points += 50;
            else if (timeForMatch < 5000) points += 25;

            // Difficulty multiplier
            points = difficultyLevel switch
            {
                DifficultyLevel.Easy => points,
                DifficultyLevel.Medium => (int)(points * 1.5),
                DifficultyLevel.Hard => (int)(points * 2.0),
                _ => points
            };

            // Hint penalty
            if (hintUsed) points = (int)(points * 0.5);

            return points;
        }

        private int CalculateStarRating(FlipCardGameSession session)
        {
            if (session.WrongAttempts == 0 && session.HintsUsed == 0) return 3;
            var accuracy = (double)session.MatchedPairs / (session.TotalMoves > 0 ? session.TotalMoves : 1);
            if (accuracy > 0.75) return 2;
            return 1;
        }

        private List<string> CheckAchievements(FlipCardGameSession session)
        {
            var achievements = new List<string>();
            if (session.WrongAttempts == 0) achievements.Add("Perfect Memory");
            if (session.HintsUsed == 0) achievements.Add("No Hints Master");
            if (session.TimeSpentSeconds < 60) achievements.Add("Speed Demon");
            return achievements;
        }

        public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int gradeId, int subjectId)
        {
            var entries = await _sessionRepository.GetLeaderboardAsync(gradeId, subjectId, 10);
            var result = entries.ToList();
            // Assign ranks
            for (int i = 0; i < result.Count; i++)
            {
                result[i].Rank = i + 1;
            }
            return result;
        }
    }
}
