using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.DragDrop;
using Nafes.API.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Student role usually
public class DragDropGameController : ControllerBase
{
    private readonly IDragDropGameService _gameService;

    public DragDropGameController(IDragDropGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<GameSessionDto>> StartGame(StartGameRequestDto request)
    {
        // int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        int studentId = 1; // Default for dev/test if auth missing
        
        try
        {
            var session = await _gameService.StartSessionAsync(request, studentId);
            return Ok(session);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("attempt")]
    public async Task<ActionResult<SubmitAttemptResponseDto>> SubmitAttempt(SubmitAttemptRequestDto request)
    {
        try
        {
            var result = await _gameService.ProcessAttemptAsync(request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("complete/{sessionId}")]
    public async Task<ActionResult<GameResultDto>> CompleteGame(int sessionId)
    {
        try
        {
            var result = await _gameService.CompleteGameAsync(sessionId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<GameSessionDto?>> GetActiveSession()
    {
        int studentId = 1; 
        var session = await _gameService.GetActiveSessionAsync(studentId);
        return Ok(session);
    }
}
