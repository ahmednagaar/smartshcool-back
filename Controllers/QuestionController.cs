using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.Question;
using Nafes.API.DTOs.Shared;
using Nafes.API.Services;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;
    private readonly ILogger<QuestionController> _logger;

    public QuestionController(IQuestionService questionService, ILogger<QuestionController> logger)
    {
        _questionService = questionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<QuestionGetDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? grade = null,
        [FromQuery] int? subject = null,
        [FromQuery] int? type = null,
        [FromQuery] int? testType = null, // Added testType parameter
        [FromQuery] int? difficulty = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = "createddate",
        [FromQuery] string? sortOrder = "desc")
    {
        var request = new QuestionSearchRequestDto
        {
            Page = page,
            PageSize = pageSize,
            Grade = grade.HasValue ? (Modules.GradeLevel)grade.Value : null,
            Subject = subject.HasValue ? (Modules.SubjectType)subject.Value : null,
            Type = type.HasValue ? (Modules.QuestionType)type.Value : null,
            TestType = testType.HasValue ? (Modules.TestType)testType.Value : null, // Mapped to DTO
            Difficulty = difficulty.HasValue ? (Modules.DifficultyLevel)difficulty.Value : null,
            SearchTerm = search,
            SortBy = sortBy,
            SortOrder = sortOrder
        };

        var response = await _questionService.GetQuestionsAsync(request);
        return Ok(response);
    }
    

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<QuestionGetDto>>> GetById(long id)
    {
        var response = await _questionService.GetQuestionByIdAsync(id);
        if (!response.Success)
            return NotFound(response);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<QuestionGetDto>>> Create([FromBody] QuestionCreateDto createDto)
    {
        // Log validation errors for debugging
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            _logger.LogWarning("Validation failed: {@Errors}", errors);
            return BadRequest(new ApiResponse<QuestionGetDto> 
            { 
                Success = false, 
                Message = "خطأ في التحقق من البيانات",
                Errors = errors.SelectMany(e => e.Value).ToList()
            });
        }

        var response = await _questionService.CreateQuestionAsync(createDto);
        if (!response.Success)
             return BadRequest(response);
             
        return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<QuestionGetDto>>> Update(long id, [FromBody] QuestionUpdateDto updateDto)
    {
        var response = await _questionService.UpdateQuestionAsync(id, updateDto);
        if (!response.Success)
        {
            if (response.Message == "السؤال غير موجود") return NotFound(response);
            return BadRequest(response);
        }
        return Ok(response);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(long id)
    {
        var response = await _questionService.DeleteQuestionAsync(id);
        if (!response.Success)
             return NotFound(response);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("bulk-import")]
    public async Task<ActionResult<ApiResponse<BulkImportResultDto>>> BulkImport([FromBody] List<QuestionCreateDto> questions)
    {
        var response = await _questionService.BulkImportQuestionsAsync(questions);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("export")]
    public async Task<ActionResult> Export(
        [FromQuery] string format = "csv",
        [FromQuery] string? search = null)
    {
        // For now, simpler client-side export is preferred, but this endpoint exists for future server-side expansion
        try 
        {
            var request = new QuestionSearchRequestDto { SearchTerm = search };
            var response = await _questionService.ExportQuestionsAsync(format, request);
            
            string contentType = format.ToLower() == "json" ? "application/json" : "text/csv";
            string fileName = $"questions_export_{DateTime.UtcNow:yyyyMMdd}.{format}";
            
            return File(response.Data!, contentType, fileName);
        }
        catch (NotImplementedException)
        {
             return BadRequest(new ApiResponse<object> { Success = false, Message = "Server-side export not implemented yet" });
        }
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<object>>> GetStats()
    {
        var response = await _questionService.GetQuestionStatsAsync();
        return Ok(response);
    }

    // Helper endpoints maintained for frontend compatibility or convenience
    [HttpGet("subjects")]
    public ActionResult GetSubjects()
    {
        var subjects = Enum.GetValues<Modules.SubjectType>()
            .Select(s => new
            {
                Id = (int)s,
                Name = s switch
                {
                    Modules.SubjectType.Arabic => "لغة عربية",
                    Modules.SubjectType.Math => "رياضيات",
                    Modules.SubjectType.Science => "علوم",
                    _ => s.ToString()
                },
                NameEn = s.ToString(),
                Icon = s switch
                {
                    Modules.SubjectType.Arabic => "📚",
                    Modules.SubjectType.Math => "🔢",
                    Modules.SubjectType.Science => "🔬",
                    _ => "📖"
                }
            })
            .ToList();

        return Ok(subjects);
    }

    [HttpGet("analytics/{id}")]
    public async Task<ActionResult> GetQuestionAnalytics(long id)
    {
        var response = await _questionService.GetQuestionAnalyticsAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response.Data);
    }
    [Authorize]
    [HttpPost("seed")]
    public async Task<ActionResult<ApiResponse<bool>>> SeedMockData()
    {
        var response = await _questionService.SeedMockQuestionsAsync();
        return Ok(response);
    }
}
