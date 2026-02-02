using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.Shared;
using Nafes.API.DTOs.Matching;
using Nafes.API.Modules;
using Nafes.API.Services;
using System.Security.Claims;

namespace Nafes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class MatchingGameController : ControllerBase
{
    private readonly IMatchingGameService _matchingGameService;

    public MatchingGameController(IMatchingGameService matchingGameService)
    {
        _matchingGameService = matchingGameService;
    }

    private long? GetCurrentStudentId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || !long.TryParse(claim.Value, out long id))
            return null; // Guest mode - return null
        return id;
    }

    [HttpPost("start")]
    public async Task<ActionResult<MatchingGameStartResponseDto>> StartGame(StartMatchingGameDto dto)
    {
        // Use authenticated student ID if available, otherwise use guest ID from frontend (or default 1)
        var authStudentId = GetCurrentStudentId();
        if (authStudentId.HasValue)
        {
            dto.StudentId = authStudentId.Value;
        }
        else if (!dto.StudentId.HasValue || dto.StudentId == 0)
        {
            dto.StudentId = 1; // Default guest student ID
        }

        try
        {
            var result = await _matchingGameService.StartGameAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("validate")]
    public async Task<ActionResult<MatchResultDto>> ValidateMatch(ValidateMatchDto dto)
    {
        try
        {
            var result = await _matchingGameService.ValidateMatchAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("hint/{sessionId}")]
    public async Task<ActionResult<HintResponseDto>> GetHint(long sessionId)
    {
        try
        {
            var result = await _matchingGameService.GetHintAsync(sessionId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("complete/{sessionId}")]
    public async Task<ActionResult<SessionCompleteDto>> CompleteSession(long sessionId)
    {
        try
        {
            var result = await _matchingGameService.CompleteSessionAsync(sessionId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<SessionCompleteDto>>> GetHistory(
        [FromQuery] GradeLevel? grade,
        [FromQuery] SubjectType? subject)
    {
        var studentId = GetCurrentStudentId() ?? 1; // Default to guest
        var result = await _matchingGameService.GetStudentHistoryAsync(studentId, grade, subject);
        return Ok(result);
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<IEnumerable<MatchingLeaderboardDto>>> GetLeaderboard(
        [FromQuery] GradeLevel grade,
        [FromQuery] SubjectType subject)
    {
        var result = await _matchingGameService.GetLeaderboardAsync(grade, subject);
        return Ok(result);
    }
}
