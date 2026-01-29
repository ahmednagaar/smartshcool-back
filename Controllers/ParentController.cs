using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs.Parent;
using Nafes.API.Modules;
using Nafes.API.Services;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ApplicationDbContext _context; // Accessing context directly for complex parent queries

    public ParentController(
        IUnitOfWork unitOfWork, 
        IPasswordHasher passwordHasher, 
        IJwtService jwtService,
        ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ParentLoginResponseDto>> Register([FromBody] ParentRegisterDto registerDto)
    {
        // 1. Verify child exists
        var child = await _unitOfWork.Students.GetByStudentCodeAsync(registerDto.ChildStudentCode);
        if (child == null)
        {
            return NotFound(new { message = "كود الطالب غير صحيح" });
        }

        // 2. Check if email exists
        var existingParent = await _context.Parents.FirstOrDefaultAsync(p => p.Email == registerDto.Email);
        if (existingParent != null)
        {
            return BadRequest(new { message = "البريد الإلكتروني مستخدم بالفعل" });
        }

        // 3. Create parent account
        var parent = new Parent
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            Phone = registerDto.Phone,
            PasswordHash = _passwordHasher.HashPassword(registerDto.Password)
        };

        // 4. Link child
        parent.Children.Add(child);
        
        // 5. Save to DB
        _context.Parents.Add(parent);
        await _context.SaveChangesAsync();

        // 6. Generate Token
        var token = _jwtService.GenerateParentToken(parent);

        return Ok(new ParentLoginResponseDto
        {
            Id = parent.Id,
            Name = parent.Name,
            Email = parent.Email,
            AccessToken = token,
            Children = parent.Children.Select(c => new ChildInfoDto
            {
                Id = c.Id,
                Name = c.Name,
                StudentCode = c.StudentCode,
                Grade = c.Grade
            }).ToList()
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<ParentLoginResponseDto>> Login([FromBody] ParentLoginDto loginDto)
    {
        var parent = await _context.Parents
            .Include(p => p.Children)
            .FirstOrDefaultAsync(p => p.Email == loginDto.Email);

        if (parent == null || !_passwordHasher.VerifyPassword(loginDto.Password, parent.PasswordHash))
        {
            return Unauthorized(new { message = "البريد الإلكتروني أو كلمة المرور غير صحيحة" });
        }

        var token = _jwtService.GenerateParentToken(parent);

        return Ok(new ParentLoginResponseDto
        {
            Id = parent.Id,
            Name = parent.Name,
            Email = parent.Email,
            AccessToken = token,
            Children = parent.Children.Select(c => new ChildInfoDto
            {
                Id = c.Id,
                Name = c.Name,
                StudentCode = c.StudentCode,
                Grade = c.Grade
            }).ToList()
        });
    }

    [Authorize(Roles = "Parent")]
    [HttpGet("child-progress/{studentId}")]
    public async Task<ActionResult<ChildProgressDto>> GetChildProgress(long studentId)
    {
        // Get parent ID from token and verify ownership
        var parentIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(parentIdClaim) || !long.TryParse(parentIdClaim, out var parentId))
        {
            return Unauthorized(new { message = "غير مصرح" }); // Unauthorized
        }

        // Verify parent owns this student
        var parent = await _context.Parents
            .Include(p => p.Children)
            .FirstOrDefaultAsync(p => p.Id == parentId);

        if (parent == null)
        {
            return Unauthorized(new { message = "ولي الأمر غير موجود" }); // Parent not found
        }

        if (!parent.Children.Any(c => c.Id == studentId))
        {
            return Forbid(); // Parent does not own this student - returns 403
        }

        var student = await _context.Students
            .Include(s => s.TestResults).ThenInclude(tr => tr.Game)
            .Include(s => s.StudentAchievements).ThenInclude(sa => sa.Achievement)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
        {
            return NotFound(new { message = "الطالب غير موجود" });
        }

        // Calculate stats
        var totalTests = student.TestResults.Count;
        var averageScore = totalTests > 0 ? student.TestResults.Average(tr => (double)tr.Score) : 0;
        
        // Simple rank placeholder (real implementation would use leaderboard service)
        var rank = 1;

        var progressDto = new ChildProgressDto
        {
            StudentId = student.Id,
            Name = student.Name,
            Grade = student.Grade,
            StudentCode = student.StudentCode,
            TotalTests = totalTests,
            AverageScore = Math.Round(averageScore, 1),
            TotalAchievements = student.StudentAchievements.Count,
            LeaderboardRank = rank,
            RecentTests = student.TestResults
                .OrderByDescending(tr => tr.CreatedDate)
                .Take(5)
                .Select(tr => new RecentTestDto
                {
                    GameTitle = tr.Game?.Title ?? "Unknown",
                    Score = tr.Score,
                    Passed = tr.Passed,
                    DateTaken = tr.CreatedDate
                }).ToList(),
            Achievements = student.StudentAchievements
                .Select(sa => new AchievementSummaryDto
                {
                    Title = sa.Achievement.Title,
                    Icon = sa.Achievement.Icon,
                    DateUnlocked = sa.DateUnlocked
                }).ToList()
        };

        return Ok(progressDto);
    }
}
