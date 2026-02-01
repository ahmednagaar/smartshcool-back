using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.MatchingGame;
using Nafes.API.DTOs.Shared;
using Nafes.API.Modules;
using Nafes.API.Services;
using System.Security.Claims;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchingQuestionController : ControllerBase
{
    private readonly IMatchingQuestionService _service;

    public MatchingQuestionController(IMatchingQuestionService service)
    {
        _service = service;
    }

    [HttpGet]
    // [Authorize] // Admin or Student can view? Usually only Admin here for management
    public async Task<ActionResult<PaginatedResponse<MatchingQuestionDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? grade = null,
        [FromQuery] int? subject = null,
        [FromQuery] string? search = null)
    {
        var (items, totalCount) = await _service.SearchAsync(
            page, 
            pageSize, 
            grade.HasValue ? (GradeLevel)grade : null, 
            subject.HasValue ? (SubjectType)subject : null, 
            search);

        return PaginatedResponse<MatchingQuestionDto>.Ok(items, page, pageSize, totalCount);
    }

    [HttpGet("{id}")]
    // [Authorize]
    public async Task<ActionResult<MatchingQuestionDto>> GetById(long id)
    {
        try
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<MatchingQuestionDto>> Create([FromBody] CreateMatchingQuestionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        var result = await _service.CreateAsync(dto, userId);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<MatchingQuestionDto>> Update(long id, [FromBody] UpdateMatchingQuestionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
            var result = await _service.UpdateAsync(id, dto, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult> Delete(long id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        await _service.DeleteAsync(id, userId);
        return NoContent();
    }

    [HttpPost("bulk-import")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult> BulkImport([FromBody] List<CreateMatchingQuestionDto> dtos)
    {
        if (dtos == null || !dtos.Any())
            return BadRequest("No data provided");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        await _service.BulkImportAsync(dtos, userId);

        return Ok(new { message = $"Successfully imported {dtos.Count} questions" });
    }
}
