using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.Data;
using Nafes.API.DTOs.Question;
using Nafes.API.DTOs.Question;
using Nafes.API.DTOs.Shared;
using Nafes.API.Modules;
using System.Text.Json;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QuestionController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<QuestionGetDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? grade = null,
        [FromQuery] int? subject = null,
        [FromQuery] int? type = null,
        [FromQuery] int? difficulty = null,
        [FromQuery] string? search = null)
    {
        // Create search DTO from query params
        var searchDto = new QuestionSearchRequestDto
        {
            Page = page,
            PageSize = Math.Min(pageSize, 100), // Max 100 per page
            Grade = grade.HasValue ? (GradeLevel)grade.Value : null,
            Subject = subject.HasValue ? (SubjectType)subject.Value : null,
            Type = type.HasValue ? (QuestionType)type.Value : null,
            Difficulty = difficulty.HasValue ? (DifficultyLevel)difficulty.Value : null,
            SearchTerm = search
        };

        var (items, totalCount) = await _unitOfWork.Questions.SearchAsync(searchDto);
        var questionDtos = _mapper.Map<IEnumerable<QuestionGetDto>>(items);

        return Ok(new PagedResult<QuestionGetDto>
        {
            Items = questionDtos,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = searchDto.PageSize
        });
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionGetDto>> GetById(long id)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null)
            return NotFound(new { message = "السؤال غير موجود" }); // Question not found

        var questionDto = _mapper.Map<QuestionGetDto>(question);
        return Ok(questionDto);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<QuestionGetDto>> Create([FromBody] QuestionCreateDto createDto)
    {
        var validationResult = ValidateQuestion(createDto.Type, createDto.Options, createDto.CorrectAnswer);
        if (!string.IsNullOrEmpty(validationResult))
        {
             return BadRequest(new { message = validationResult });
        }

        var question = _mapper.Map<Question>(createDto);
        await _unitOfWork.Questions.AddAsync(question);
        await _unitOfWork.CommitAsync();

        var questionDto = _mapper.Map<QuestionGetDto>(question);
        return CreatedAtAction(nameof(GetById), new { id = question.Id }, questionDto);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<QuestionGetDto>> Update(long id, [FromBody] QuestionUpdateDto updateDto)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null)
            return NotFound(new { message = "السؤال غير موجود" });

        var validationResult = ValidateQuestion(updateDto.Type, updateDto.Options, updateDto.CorrectAnswer);
        if (!string.IsNullOrEmpty(validationResult))
        {
             return BadRequest(new { message = validationResult });
        }

        _mapper.Map(updateDto, question);
        _unitOfWork.Questions.Update(question);
        await _unitOfWork.CommitAsync();

        var questionDto = _mapper.Map<QuestionGetDto>(question);
        return Ok(questionDto);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null)
            return NotFound(new { message = "السؤال غير موجود" });

        _unitOfWork.Questions.Remove(question);
        await _unitOfWork.CommitAsync();

        return Ok(new { message = "تم حذف السؤال بنجاح" }); // Question deleted successfully
    }

    [HttpGet("difficulty/{difficulty}")]
    public async Task<ActionResult<IEnumerable<QuestionGetDto>>> GetByDifficulty(DifficultyLevel difficulty)
    {
        var questions = await _unitOfWork.Questions.GetByDifficultyAsync(difficulty);
        var questionDtos = _mapper.Map<IEnumerable<QuestionGetDto>>(questions);
        return Ok(questionDtos);
    }

    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<QuestionGetDto>>> GetByType(QuestionType type)
    {
        var questions = await _unitOfWork.Questions.GetByTypeAsync(type);
        var questionDtos = _mapper.Map<IEnumerable<QuestionGetDto>>(questions);
        return Ok(questionDtos);
    }

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<QuestionGetDto>>> GetFiltered(
        [FromQuery] int grade,
        [FromQuery] int subject,
        [FromQuery] int testType)
    {
        var gradeLevel = (GradeLevel)grade;
        var subjectType = (SubjectType)subject;
        var testTypeEnum = (TestType)testType;

        var questions = await _unitOfWork.Questions.GetFilteredAsync(gradeLevel, subjectType, testTypeEnum);
        var questionDtos = _mapper.Map<IEnumerable<QuestionGetDto>>(questions);
        return Ok(questionDtos);
    }

    [HttpPost("search")]
    public async Task<ActionResult<PagedResult<QuestionGetDto>>> Search([FromBody] QuestionSearchRequestDto searchDto)
    {
        var (items, totalCount) = await _unitOfWork.Questions.SearchAsync(searchDto);
        var questionDtos = _mapper.Map<IEnumerable<QuestionGetDto>>(items);

        return Ok(new PagedResult<QuestionGetDto>
        {
            Items = questionDtos,
            TotalCount = totalCount,
            PageNumber = searchDto.Page,
            PageSize = searchDto.PageSize
        });
    }

    /// <summary>
    /// Get question counts grouped by grade and subject for dashboard
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult> GetStats()
    {
        var questions = await _unitOfWork.Questions.GetAllAsync();
        var activeQuestions = questions.Where(q => !q.IsDeleted).ToList();

        var stats = new
        {
            TotalCount = activeQuestions.Count,
            ByGrade = new[] { 3, 4, 5, 6 }.Select(g => new
            {
                Grade = g,
                Count = activeQuestions.Count(q => (int)q.Grade == g),
                BySubject = new[] { 1, 2, 3 }.Select(s => new
                {
                    Subject = s,
                    SubjectName = s == 1 ? "لغة عربية" : s == 2 ? "رياضيات" : "علوم",
                    Count = activeQuestions.Count(q => (int)q.Grade == g && (int)q.Subject == s)
                }).ToList()
            }).ToList()
        };

        return Ok(stats);
    }

    [Authorize]
    [HttpPost("restore/{id}")]
    public async Task<ActionResult> Restore(long id)
    {
        var question = await _unitOfWork.Questions.GetIncludeDeletedAsync(id);
        if (question == null)
            return NotFound(new { message = "السؤال غير موجود" });

        question.IsDeleted = false;
        // Optional: Update DeletedAt to null or keep history? 
        // BaseModel usually keeps DeletedAt. Let's just unset IsDeleted.
        // If DeletedAt is nullable, set to null.
        question.DeletedAt = null; 
        
        _unitOfWork.Questions.Update(question);
        await _unitOfWork.CommitAsync();

        return Ok(new { message = "تم استعادة السؤال بنجاح" });
    }

    [HttpGet("analytics/{id}")]
    public async Task<ActionResult> GetQuestionAnalytics(long id)
    {
        // Future: Calculate real stats from TestResults
        // For now return basic info or placeholders
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null) return NotFound();

        // Mock data for UI development
        var analytics = new 
        {
            Id = id,
            UsageCount = new Random().Next(10, 500),
            SuccessRate = new Random().Next(60, 95),
            AvgTimeSeconds = new Random().Next(15, 120),
            DifficultyRating = question.Difficulty.ToString()
        };

        return Ok(analytics);
    }

    private string? ValidateQuestion(QuestionType type, string? options, string? correctAnswer)
    {
        if (type == QuestionType.MultipleChoice || type == QuestionType.TrueFalse || type == QuestionType.FillInTheBlank)
        {
            if (string.IsNullOrWhiteSpace(options))
            {
                return "الخيارات مطلوبة لهذا النوع من الأسئلة"; // Options required
            }

            try 
            {
                // Verify partial JSON structure mainly to ensure it's valid JSON
                // You can add more specific schema validation here if needed
                JsonDocument.Parse(options);
            }
            catch
            {
                return "تنسيق الخيارات غير صالح (JSON)"; // Invalid JSON
            }

            if (string.IsNullOrWhiteSpace(correctAnswer))
            {
                return "الإجابة الصحيحة مطلوبة"; // Correct answer required
            }
        }
        
        return null;
    }
}

