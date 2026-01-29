using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs.Achievement;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AchievementController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AchievementController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AchievementDto>>> GetAll()
    {
        var achievements = await _context.Achievements.Where(a => !a.IsDeleted).ToListAsync();
        return Ok(_mapper.Map<IEnumerable<AchievementDto>>(achievements));
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<AchievementDto>>> GetByStudent(long studentId)
    {
        var allAchievements = await _context.Achievements.Where(a => !a.IsDeleted).ToListAsync();
        var studentAchievements = await _context.StudentAchievements
            .Where(sa => sa.StudentId == studentId && !sa.IsDeleted)
            .ToListAsync();

        var result = allAchievements.Select(a => new AchievementDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            Icon = a.Icon,
            Points = a.Points,
            IsUnlocked = studentAchievements.Any(sa => sa.AchievementId == a.Id),
            DateUnlocked = studentAchievements.FirstOrDefault(sa => sa.AchievementId == a.Id)?.DateUnlocked
        });

        return Ok(result);
    }
}
