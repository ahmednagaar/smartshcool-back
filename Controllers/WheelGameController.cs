using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.WheelGame;
using Nafes.API.Services;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Uncomment when auth is live
public class WheelGameController : ControllerBase
{
    private readonly IWheelGameService _service;
    private readonly ILogger<WheelGameController> _logger;

    public WheelGameController(IWheelGameService service, ILogger<WheelGameController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<ActionResult<StartGameResponseDto>> StartGame([FromBody] StartWheelGameDto dto)
    {
        _logger.LogInformation("StartGame Request: StudentId={StudentId}, GradeId={GradeId}, SubjectId={SubjectId}, TestType={TestType}", 
            dto.StudentId, dto.GradeId, dto.SubjectId, dto.TestType);
        try
        {
            var result = await _service.StartGameResponseAsync(dto);
            _logger.LogInformation("StartGame Success: SessionId={SessionId}, TotalQuestions={Count}", result.SessionId, result.TotalQuestions);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("StartGame Failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StartGame Error: Unhandled exception");
            return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    [HttpPost("spin")]
    public async Task<ActionResult<SpinResultDto>> SpinWheel([FromBody] SpinWheelDto dto)
    {
        try
        {
            return Ok(await _service.SpinWheelAsync(dto));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Session not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("submit")]
    public async Task<ActionResult<AnswerResultDto>> SubmitAnswer([FromBody] SubmitAnswerDto dto)
    {
        try
        {
            return Ok(await _service.SubmitAnswerAsync(dto));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Session or Question not found" });
        }
    }

    [HttpPost("hint")]
    public async Task<ActionResult<HintResponseDto>> GetHint([FromBody] GetHintDto dto)
    {
        try
        {
            return Ok(await _service.GetHintAsync(dto));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboard(
        [FromQuery] int gradeId, 
        [FromQuery] int subjectId)
    {
        return Ok(await _service.GetLeaderboardAsync(gradeId, subjectId));
    }

    [HttpGet("stats/{studentId}")]
    public async Task<ActionResult<StudentStatisticsDto>> GetStats(long studentId)
    {
        return Ok(await _service.GetStudentStatsAsync(studentId));
    }
}
