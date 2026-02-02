using System.Threading.Tasks;
using Nafes.API.DTOs.DragDrop;

namespace Nafes.API.Services;

public interface IDragDropGameService
{
    Task<GameSessionDto> StartSessionAsync(StartGameRequestDto request, int studentId);
    Task<SubmitAttemptResponseDto> ProcessAttemptAsync(SubmitAttemptRequestDto request);
    Task<GameResultDto> CompleteGameAsync(int sessionId);
    Task<GameSessionDto?> GetActiveSessionAsync(int studentId);
}
