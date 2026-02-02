using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.DragDrop;
using Nafes.API.DTOs.Shared;
using Nafes.API.Modules; // For Enums
using Nafes.API.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin,SuperAdmin")] // Uncomment when Auth is fully verified or if testing without
public class DragDropQuestionController : ControllerBase
{
    private readonly IDragDropQuestionService _service;
    private readonly IUIThemeService _themeService;

    public DragDropQuestionController(IDragDropQuestionService service, IUIThemeService themeService)
    {
        _service = service;
        _themeService = themeService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<DragDropQuestionDto>>> GetAll([FromQuery] PaginationParams paginationParams, [FromQuery] GradeLevel? grade, [FromQuery] SubjectType? subject)
    {
        var result = await _service.GetAllPaginatedAsync(paginationParams, grade, subject);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DragDropQuestionDto>> GetById(int id)
    {
        var result = await _service.GetQuestionByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<DragDropQuestionDto>> Create(CreateDragDropQuestionDto dto)
    {
        // Get Admin ID from claims
        // int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        int userId = 1; // Validation Phase hardcode if auth not ready in test env
        
        var result = await _service.CreateQuestionAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DragDropQuestionDto>> Update(int id, UpdateDragDropQuestionDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        int userId = 1;

        try
        {
            var result = await _service.UpdateQuestionAsync(dto, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _service.DeleteQuestionAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("themes")]
    public ActionResult<IEnumerable<UITheme>> GetThemes()
    {
        return Ok(_themeService.GetAvailableThemes());
    }
}
