using System.Threading.Tasks;
using Nafes.API.DTOs.FlipCard;
using Nafes.API.DTOs.TestResult;
using Nafes.API.Modules;

namespace Nafes.API.Services.FlipCard
{
    public interface IFlipCardGameService
    {
        Task<FlipCardGameSession> StartSessionAsync(StartFlipCardGameDto dto);
        Task<MatchResultDto> RecordMatchAsync(RecordMatchDto dto);
        Task<object> RecordWrongMatchAsync(RecordWrongMatchDto dto); // Returns penalty info
        Task<object> GetHintAsync(GetHintDto dto); // Returns hint info
        Task<SessionCompleteDto> CompleteSessionAsync(int sessionId);
        Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int gradeId, int subjectId);
    }
}
