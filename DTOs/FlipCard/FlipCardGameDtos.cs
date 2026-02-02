using System.Collections.Generic;
using Nafes.API.Modules;

namespace Nafes.API.DTOs.FlipCard
{
    public class StartFlipCardGameDto
    {
        public int StudentId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int? QuestionId { get; set; }
        public int? DifficultyLevel { get; set; }
        public FlipCardGameMode? GameMode { get; set; }
    }

    public class GameCardDto
    {
        public string Id { get; set; } // "card-{pairId}-{num}"
        public int PairId { get; set; }
        public int CardNumber { get; set; }
        public FlipCardContentType Type { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string AudioUrl { get; set; }
    }

    public class RecordMatchDto
    {
        public int SessionId { get; set; }
        public int PairId { get; set; }
        public int Card1FlippedAtMs { get; set; }
        public int Card2FlippedAtMs { get; set; }
        public int AttemptsBeforeMatch { get; set; }
        public bool HintUsed { get; set; }
    }

    public class RecordWrongMatchDto
    {
        public int SessionId { get; set; }
        public string Card1Id { get; set; } = string.Empty;
        public string Card2Id { get; set; } = string.Empty;
    }

    public class GetHintDto
    {
        public int SessionId { get; set; }
    }

    public class MatchResultDto
    {
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
        public int TotalScore { get; set; }
        public int MatchedPairs { get; set; }
        public int TotalPairs { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public bool IsGameComplete { get; set; }
    }

    public class SessionCompleteDto
    {
        public int SessionId { get; set; }
        public int FinalScore { get; set; }
        public int MatchedPairs { get; set; }
        public int TotalPairs { get; set; }
        public int TotalMoves { get; set; }
        public int TimeSpent { get; set; }
        public int StarRating { get; set; }
        public int Rank { get; set; }
        public List<AttemptDetailDto> Attempts { get; set; } = new();
        public List<string> Achievements { get; set; } = new();
    }

    public class AttemptDetailDto
    {
        public int PairId { get; set; }
        public bool IsCorrect { get; set; }
        public int Points { get; set; }
        public int TimeMs { get; set; }
    }
}
