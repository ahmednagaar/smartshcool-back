using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.Data;
using Nafes.API.DTOs.Game;
using Nafes.API.Modules;
using Nafes.API.DTOs.Shared;
using Microsoft.IdentityModel.Tokens;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public GameController(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameGetDto>>> GetAll([FromQuery] int? grade, [FromQuery] int? subject)
    {
        var games = await _unitOfWork.Games.GetAllAsync();
        
        if (grade.HasValue)
        {
            games = games.Where(g => (int)g.Grade == grade.Value);
        }

        if (subject.HasValue)
        {
            games = games.Where(g => (int)g.Subject == subject.Value);
        }

        var gameDtos = _mapper.Map<IEnumerable<GameGetDto>>(games);
        return Ok(gameDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameGetDto>> GetById(long id)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(id);
        if (game == null)
            return NotFound(new { message = "اللعبة غير موجودة" }); // Game not found

        var gameDto = _mapper.Map<GameGetDto>(game);
        return Ok(gameDto);
    }

    [HttpGet("{id}/with-questions")]
    public async Task<ActionResult<GameWithQuestionsDto>> GetWithQuestions(long id)
    {
        var game = await _unitOfWork.Games.GetWithQuestionsAsync(id);
        if (game == null)
            return NotFound(new { message = "اللعبة غير موجودة" });

        var gameDto = _mapper.Map<GameWithQuestionsDto>(game);
        return Ok(gameDto);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<GameGetDto>> Create([FromBody] GameCreateDto createDto)
    {
        var game = new Game
        {
            Title = createDto.Title,
            Description = createDto.Description,
            TimeLimit = createDto.TimeLimit,
            PassingScore = createDto.PassingScore,
            Grade = createDto.Grade,
            Subject = createDto.Subject
        };

        await _unitOfWork.Games.AddAsync(game);
        await _unitOfWork.CommitAsync();

        // Add questions to game
        if (createDto.QuestionIds.Any())
        {
            var order = 1;
            foreach (var questionId in createDto.QuestionIds)
            {
                var gameQuestion = new GameQuestion
                {
                    GameId = game.Id,
                    QuestionId = questionId,
                    Order = order++
                };
                await _context.GameQuestions.AddAsync(gameQuestion);
            }
            await _unitOfWork.CommitAsync();
        }

        var gameDto = _mapper.Map<GameGetDto>(game);
        return CreatedAtAction(nameof(GetById), new { id = game.Id }, gameDto);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<GameGetDto>> Update(long id, [FromBody] GameUpdateDto updateDto)
    {
        var game = await _unitOfWork.Games.GetWithQuestionsAsync(id);
        if (game == null)
            return NotFound(new { message = "اللعبة غير موجودة" });

        game.Title = updateDto.Title;
        game.Description = updateDto.Description;
        game.TimeLimit = updateDto.TimeLimit;
        game.PassingScore = updateDto.PassingScore;
        game.Grade = updateDto.Grade;
        game.Subject = updateDto.Subject;

        // Remove old questions
        foreach (var gq in game.GameQuestions.ToList())
        {
            _context.GameQuestions.Remove(gq);
        }

        // Add new questions
        if (updateDto.QuestionIds.Any())
        {
            var order = 1;
            foreach (var questionId in updateDto.QuestionIds)
            {
                var gameQuestion = new GameQuestion
                {
                    GameId = game.Id,
                    QuestionId = questionId,
                    Order = order++
                };
                await _context.GameQuestions.AddAsync(gameQuestion);
            }
        }

        _unitOfWork.Games.Update(game);
        await _unitOfWork.CommitAsync();

        var gameDto = _mapper.Map<GameGetDto>(game);
        return Ok(gameDto);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(id);
        if (game == null)
            return NotFound(new { message = "اللعبة غير موجودة" });

        _unitOfWork.Games.Remove(game);
        await _unitOfWork.CommitAsync();

        return Ok(new { message = "تم حذف اللعبة بنجاح" });
    }

    [HttpGet("{id}/available-questions")]
    public async Task<ActionResult<IEnumerable<GameQuestionDto>>> GetAvailableQuestions(long id, [FromQuery] string? search = null)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(id);
        if (game == null)
            return NotFound(new { message = "اللعبة غير موجودة" });

        var questions = await _unitOfWork.Questions.GetAvailableForGameAsync(id, game.Grade, game.Subject, search);
        
        // Map to DTO - reusing GameQuestionDto but Order will be 0
        var availableDtos = questions.Select(q => new GameQuestionDto
        {
            QuestionId = q.Id,
            Text = q.Text,
            Type = q.Type.ToString(),
            Difficulty = q.Difficulty.ToString(),
            MediaUrl = q.MediaUrl,
            Options = q.Options,
            Order = 0
        });

        return Ok(availableDtos);
    }

    [Authorize]
    [HttpPost("{id}/questions")]
    public async Task<ActionResult> AddQuestions(long id, [FromBody] AddQuestionsDto addDto)
    {
        var game = await _unitOfWork.Games.GetWithQuestionsAsync(id);
        if (game == null)
            return NotFound(new { message = "اللعبة غير موجودة" });

        var currentMaxOrder = game.GameQuestions.Any() ? game.GameQuestions.Max(gq => gq.Order) : 0;
        var addedCount = 0;

        foreach (var questionId in addDto.QuestionIds)
        {
            // Verify question exists and matches grade/subject
            var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
            if (question == null) continue;

            // Strict validation: must match game's grade and subject
            if (question.Grade != game.Grade || question.Subject != game.Subject)
            {
                // Verify if we want to skip or error. Using skip for partial success or error?
                // For "professional" feel, maybe we should error if ANY don't match?
                // Or just skip and return warning. usage: skipping for now.
                continue;
            }

            // Check if already exists
            if (game.GameQuestions.Any(gq => gq.QuestionId == questionId))
                continue;

            currentMaxOrder++;
            var gameQuestion = new GameQuestion
            {
                GameId = game.Id,
                QuestionId = questionId,
                Order = currentMaxOrder
            };
            
            await _context.GameQuestions.AddAsync(gameQuestion);
            addedCount++;
        }

        if (addedCount > 0)
        {
            await _unitOfWork.CommitAsync();
            return Ok(new { message = $"تم إضافة {addedCount} سؤال بنجاح" });
        }

        return BadRequest(new { message = "لم يتم إضافة أي أسئلة. تأكد من أن الأسئلة تطابق الصف والمادة ولم يتم إضافتها مسبقاً." });
    }

    [Authorize]
    [HttpDelete("{id}/questions")]
    public async Task<ActionResult> RemoveQuestions(long id, [FromBody] AddQuestionsDto removeDto)
    {
        var game = await _unitOfWork.Games.GetWithQuestionsAsync(id);
        if (game == null)
            return NotFound(new { message = "اللعبة غير موجودة" });

        var toRemove = game.GameQuestions.Where(gq => removeDto.QuestionIds.Contains(gq.QuestionId)).ToList();
        
        if (!toRemove.Any())
        {
            return NotFound(new { message = "الأسئلة المحددة غير موجودة في هذه اللعبة" });
        }

        _context.GameQuestions.RemoveRange(toRemove);
        await _unitOfWork.CommitAsync();

        return Ok(new { message = "تم إزالة الأسئلة بنجاح" });
    }

    [Authorize]
    [HttpPut("{id}/reorder")]
    public async Task<ActionResult> ReorderQuestions(long id, [FromBody] ReorderQuestionsDto reorderDto)
    {
        var game = await _unitOfWork.Games.GetWithQuestionsAsync(id);
        if (game == null)
            return NotFound(new { message = "اللعبة غير موجودة" });

        foreach (var item in reorderDto.Questions)
        {
            var gameQuestion = game.GameQuestions.FirstOrDefault(gq => gq.QuestionId == item.QuestionId);
            if (gameQuestion != null)
            {
                gameQuestion.Order = item.Order;
                _context.GameQuestions.Update(gameQuestion);
            }
        }

        await _unitOfWork.CommitAsync();
        return Ok(new { message = "تم إعادة ترتيب الأسئلة بنجاح" });
    }
}

