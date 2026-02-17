using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nafes.API.DTOs.Shared; // For PaginationParams
using Nafes.API.DTOs.FlipCard;
using Nafes.API.Services.FlipCard;

namespace Nafes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlipCardQuestionController : ControllerBase
    {
        private readonly IFlipCardQuestionService _service;
        private readonly ILogger<FlipCardQuestionController> _logger;

        public FlipCardQuestionController(IFlipCardQuestionService service, ILogger<FlipCardQuestionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("grade/{gradeId}/subject/{subjectId}")]
        [AllowAnonymous] // Students play without accounts
        public async Task<IActionResult> GetByGradeAndSubject(int gradeId, int subjectId)
        {
            var questions = await _service.GetByGradeAndSubjectAsync(gradeId, subjectId);

            // For students: don't reveal pair mappings (logic usually handled in game start, but listed here too)
            if (User.IsInRole("Student"))
            {
                // Return metadata only
                return Ok(questions.Select(q => new
                {
                    q.Id,
                    q.GameTitle,
                    q.Instructions,
                    q.NumberOfPairs,
                    q.DifficultyLevel,
                    q.GameMode, // e.g. "Both"
                    q.Category
                }));
            }

            // For admins: full data
            return Ok(questions);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _service.GetByIdAsync(id, includePairs: true);
            if (question == null) return NotFound();
            return Ok(question);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateFlipCardQuestionDto dto)
        {
            try
            {
                var question = await _service.CreateQuestionAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = question.Id }, question);
            }
            catch (Exception ex) // Catch ValidationException if defined
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlipCardQuestionDto dto)
        {
            if (id != dto.Id) return BadRequest("ID Mismatch");

            try
            {
                var question = await _service.UpdateQuestionAsync(id, dto);
                return Ok(question);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Question not found") return NotFound();
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("paginated")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAllPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, [FromQuery] string? searchTerm = null)
        {
            var result = await _service.GetAllPaginatedAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }


        [HttpGet("count")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestionCount(int gradeId, int subjectId)
        {
            var count = await _service.GetQuestionCountAsync(gradeId, subjectId);
            return Ok(new { count });
        }

        [HttpGet("categories")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetCategories(int? gradeId, int? subjectId)
        {
            var categories = await _service.GetCategoriesAsync(gradeId, subjectId);
            return Ok(categories);
        }
    }
}
