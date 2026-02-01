using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.MatchingGame;
using Nafes.API.Modules;
using Nafes.API.Services;
using System.Security.Claims;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchingGameController : ControllerBase
{
    private readonly IMatchingGameService _gameService;

    public MatchingGameController(IMatchingGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("start")]
    // [Authorize]
    public async Task<ActionResult<GameStartResponseDto>> StartGame([FromBody] StartGameDto dto)
    {
        try
        {
            var result = await _gameService.StartGameAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("submit")]
    // [Authorize]
    public async Task<ActionResult<GameResultDto>> SubmitGame([FromBody] SubmitMatchingGameDto dto)
    {
        try
        {
            var result = await _gameService.SubmitGameAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<IEnumerable<MatchingLeaderboardDto>>> GetLeaderboard(
        [FromQuery] int grade, 
        [FromQuery] int subject, 
        [FromQuery] int top = 10)
    {
        var result = await _gameService.GetLeaderboardAsync((GradeLevel)grade, (SubjectType)subject, top);
        return Ok(result);
    }
    
    [HttpGet("history/{studentId}")]
    // [Authorize]
    public async Task<ActionResult<IEnumerable<GameResultDto>>> GetHistory(
        long studentId,
        [FromQuery] int? grade = null,
        [FromQuery] int? subject = null)
    {
        // Security check: simple check if user is requesting their own history or is admin
        // var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (currentUserId != studentId.ToString() && !User.IsInRole("Admin")) ...
        
        var result = await _gameService.GetStudentHistoryAsync(
            studentId, 
            grade.HasValue ? (GradeLevel)grade : null, 
            subject.HasValue ? (SubjectType)subject : null);
            
        return Ok(result);
    }
}
