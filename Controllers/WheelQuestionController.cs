using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.Shared;
using Nafes.API.DTOs.WheelGame;
using Nafes.API.Modules;
using Nafes.API.Services;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WheelQuestionController : ControllerBase
{
    private readonly IWheelQuestionService _service;
    private readonly ILogger<WheelQuestionController> _logger;

    public WheelQuestionController(IWheelQuestionService service, ILogger<WheelQuestionController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Helper to get the admin username from JWT claims.
    /// </summary>
    private string GetAdminUsername()
    {
        return User?.FindFirst(ClaimTypes.Name)?.Value
            ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User?.FindFirst("username")?.Value
            ?? "admin";
    }

    // ✅ READ endpoints — open to everyone (students need to load questions)

    [AllowAnonymous]
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

    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories(
        [FromQuery] GradeLevel? grade = null, 
        [FromQuery] SubjectType? subject = null)
    {
        return Ok(await _service.GetCategoriesAsync(grade, subject));
    }

    // 🔒 WRITE endpoints — admin only

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost]
    public async Task<ActionResult<WheelQuestionResponseDto>> Create([FromBody] CreateWheelQuestionDto dto)
    {
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
            var userId = GetAdminUsername();
            var result = await _service.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wheel question");
            return StatusCode(500, new
            {
                success = false,
                message = "حدث خطأ في الخادم، يرجى المحاولة مرة أخرى"
            });
        }
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<WheelQuestionResponseDto>> Update(long id, [FromBody] UpdateWheelQuestionDto dto)
    {
        try
        {
            var userId = GetAdminUsername();
            return Ok(await _service.UpdateAsync(id, dto, userId));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        try
        {
            var userId = GetAdminUsername();
            await _service.DeleteAsync(id, userId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost("bulk-import")]
    public async Task<ActionResult> BulkImport([FromBody] List<CreateWheelQuestionDto> dtos)
    {
        var userId = GetAdminUsername();
        await _service.BulkImportAsync(dtos, userId);
        return Ok(new { message = $"Imported {dtos.Count} questions successfully" });
    }
}
