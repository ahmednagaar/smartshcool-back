using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.Shared;
using Nafes.API.DTOs.Matching;
using Nafes.API.Modules;
using Nafes.API.Services;

namespace Nafes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous] // Dev mode - remove for production
public class MatchingGameAdminController : ControllerBase
{
    private readonly IMatchingGameService _matchingGameService;

    public MatchingGameAdminController(IMatchingGameService matchingGameService)
    {
        _matchingGameService = matchingGameService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MatchingGameDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] GradeLevel? grade = null,
        [FromQuery] SubjectType? subject = null)
    {
        var result = await _matchingGameService.GetGamesAsync(page, pageSize, grade, subject);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MatchingGameDto>> GetById(long id)
    {
        try
        {
            var game = await _matchingGameService.GetGameByIdAsync(id);
            return Ok(game);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<MatchingGameDto>> Create(CreateMatchingGameDto dto)
    {
        var game = await _matchingGameService.CreateGameAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MatchingGameDto>> Update(long id, UpdateMatchingGameDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");

        try
        {
            var game = await _matchingGameService.UpdateGameAsync(id, dto);
            return Ok(game);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var result = await _matchingGameService.DeleteGameAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
