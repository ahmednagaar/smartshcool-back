using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.Shared;
using Nafes.API.DTOs.WheelGame;
using Nafes.API.Modules;
using Nafes.API.Services;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Dev mode
public class WheelQuestionController : ControllerBase
{
    private readonly IWheelQuestionService _service;

    public WheelQuestionController(IWheelQuestionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<WheelQuestionResponseDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] GradeLevel? grade = null,
        [FromQuery] SubjectType? subject = null,
        [FromQuery] DifficultyLevel? difficulty = null,
        [FromQuery] string? category = null,
        [FromQuery] string? search = null)
    {
        var (items, totalCount) = await _service.SearchAsync(page, pageSize, grade, subject, difficulty, category, search);
        return PaginatedResponse<WheelQuestionResponseDto>.Ok(items, page, pageSize, totalCount);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WheelQuestionResponseDto>> GetById(long id)
    {
        try
        {
            return Ok(await _service.GetByIdAsync(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<WheelQuestionResponseDto>> Create([FromBody] CreateWheelQuestionDto dto)
    {
        // Return detailed validation errors
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
            );
            return BadRequest(new { message = "Validation failed", errors });
        }

        try
        {
            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
            var userId = "system"; // Temp
            var result = await _service.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace, inner = ex.InnerException?.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<WheelQuestionResponseDto>> Update(long id, [FromBody] UpdateWheelQuestionDto dto)
    {
        try
        {
            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
            var userId = "system"; // Temp
            return Ok(await _service.UpdateAsync(id, dto, userId));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        try
        {
            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
            var userId = "system";
            await _service.DeleteAsync(id, userId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("bulk-import")]
    public async Task<ActionResult> BulkImport([FromBody] List<CreateWheelQuestionDto> dtos)
    {
        var userId = "system";
        await _service.BulkImportAsync(dtos, userId);
        return Ok(new { message = $"Imported {dtos.Count} questions successfully" });
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories(
        [FromQuery] GradeLevel? grade = null, 
        [FromQuery] SubjectType? subject = null)
    {
        return Ok(await _service.GetCategoriesAsync(grade, subject));
    }
}
