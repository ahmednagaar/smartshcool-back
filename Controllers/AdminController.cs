using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.Data;
using Nafes.API.DTOs.Admin;
using Nafes.API.Modules;
using Nafes.API.Services;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public AdminController(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AdminLoginResponseDto>> Login([FromBody] AdminLoginDto loginDto)
    {
        var admin = await _unitOfWork.Admins.GetByUsernameAsync(loginDto.Username);
        
        if (admin == null)
        {
            return Unauthorized(new { message = "اسم المستخدم أو كلمة المرور غير صحيحة" }); // Invalid credentials
        }

        // Check if account is locked
        if (admin.IsLocked && admin.LockedUntil.HasValue && admin.LockedUntil > DateTime.UtcNow)
        {
            return Unauthorized(new { message = "الحساب مقفل. حاول مرة أخرى لاحقاً" }); // Account locked
        }

        // Check if account is approved
        if (!admin.IsApproved)
        {
            return Unauthorized(new { message = "الحساب في انتظار الموافقة من المسؤول" }); // Pending approval
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(loginDto.Password, admin.PasswordHash))
        {
            // Increment failed login attempts
            admin.FailedLoginAttempts++;
            
            if (admin.FailedLoginAttempts >= 5)
            {
                admin.IsLocked = true;
                admin.LockedUntil = DateTime.UtcNow.AddMinutes(30); // Lock for 30 minutes
            }
            
            _unitOfWork.Admins.Update(admin);
            await _unitOfWork.CommitAsync();
            
            return Unauthorized(new { message = "اسم المستخدم أو كلمة المرور غير صحيحة" });
        }

        // Reset failed attempts and unlock if needed
        admin.FailedLoginAttempts = 0;
        admin.IsLocked = false;
        admin.LockedUntil = null;
        admin.LastLogin = DateTime.UtcNow;
        
        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(admin);
        var refreshTokenString = _jwtService.GenerateRefreshToken();
        
        // Save refresh token
        var refreshToken = new RefreshToken
        {
            Token = refreshTokenString,
            AdminId = admin.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
        
        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        _unitOfWork.Admins.Update(admin);
        await _unitOfWork.CommitAsync();

        return Ok(new AdminLoginResponseDto
        {
            Id = admin.Id,
            Username = admin.Username,
            Email = admin.Email,
            Role = admin.Role.ToString(),
            AccessToken = accessToken,
            RefreshToken = refreshTokenString
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);
        
        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "رمز التحديث غير صالح" }); // Invalid refresh token
        }

        var admin = refreshToken.Admin;
        
        // Generate new tokens
        var newAccessToken = _jwtService.GenerateAccessToken(admin);
        var newRefreshTokenString = _jwtService.GenerateRefreshToken();
        
        // Revoke old refresh token
        refreshToken.IsRevoked = true;
        refreshToken.RevokedDate = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.Update(refreshToken);
        
        // Create new refresh token
        var newRefreshToken = new RefreshToken
        {
            Token = newRefreshTokenString,
            AdminId = admin.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
        
        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
        await _unitOfWork.CommitAsync();

        return Ok(new RefreshTokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenString
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);
        
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedDate = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(refreshToken);
            await _unitOfWork.CommitAsync();
        }

        return Ok(new { message = "تم تسجيل الخروج بنجاح" }); // Logged out successfully
    }

    [Authorize]
    [HttpGet("dashboard-stats")]
    public async Task<ActionResult> GetDashboardStats()
    {
        var totalStudents = (await _unitOfWork.Students.GetAllAsync()).Count();
        var totalQuestions = (await _unitOfWork.Questions.GetAllAsync()).Count();
        var totalGames = (await _unitOfWork.Games.GetAllAsync()).Count();
        var totalTests = (await _unitOfWork.TestResults.GetAllAsync()).Count();

        return Ok(new
        {
            totalStudents,
            totalQuestions,
            totalGames,
            totalTests
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] AdminRegisterDto registerDto)
    {
        // Check if username already exists
        var existingAdmin = await _unitOfWork.Admins.GetByUsernameAsync(registerDto.Username);
        if (existingAdmin != null)
        {
            return BadRequest(new { message = "اسم المستخدم موجود بالفعل" }); // Username already exists
        }

        var admin = new Admin
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = _passwordHasher.HashPassword(registerDto.Password),
            Role = AdminRole.Admin,
            IsApproved = false // Requires SuperAdmin approval
        };

        await _unitOfWork.Admins.AddAsync(admin);
        await _unitOfWork.CommitAsync();

        return Ok(new { message = "تم إنشاء الحساب بنجاح. في انتظار موافقة المسؤول", id = admin.Id });
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<AdminGetDto>>> GetPendingAdmins()
    {
        var admins = await _unitOfWork.Admins.GetAllAsync();
        var pendingAdmins = admins.Where(a => !a.IsApproved);
        
        var result = pendingAdmins.Select(a => new AdminGetDto
        {
            Id = a.Id,
            Username = a.Username,
            Email = a.Email,
            Role = a.Role.ToString(),
            IsApproved = a.IsApproved,
            CreatedDate = a.CreatedDate
        });

        return Ok(result);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("approve/{id}")]
    public async Task<ActionResult> ApproveAdmin(long id)
    {
        var admin = await _unitOfWork.Admins.GetByIdAsync(id);
        if (admin == null)
        {
            return NotFound(new { message = "المسؤول غير موجود" }); // Admin not found
        }

        admin.IsApproved = true;
        _unitOfWork.Admins.Update(admin);
        
        // Audit Log
        var currentAdminId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var currentAdminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
        await _unitOfWork.AuditLogs.LogAsync("Approve", "Admin", admin.Id.ToString(), $"Approved admin: {admin.Username}", currentAdminId, currentAdminName);

        await _unitOfWork.CommitAsync();

        return Ok(new { message = "تم الموافقة على المسؤول بنجاح" }); // Admin approved
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("reject/{id}")]
    public async Task<ActionResult> RejectAdmin(long id)
    {
        var admin = await _unitOfWork.Admins.GetByIdAsync(id);
        if (admin == null)
        {
            return NotFound(new { message = "المسؤول غير موجود" });
        }

        if (admin.IsApproved)
        {
            return BadRequest(new { message = "لا يمكن رفض مسؤول معتمد" }); // Cannot reject approved admin
        }

        _unitOfWork.Admins.Remove(admin);
        
        // Audit Log
        var currentAdminId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var currentAdminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
        await _unitOfWork.AuditLogs.LogAsync("Reject", "Admin", admin.Id.ToString(), $"Rejected admin request: {admin.Username}", currentAdminId, currentAdminName);

        await _unitOfWork.CommitAsync();

        return Ok(new { message = "تم رفض طلب المسؤول" }); // Admin request rejected
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<AdminGetDto>>> GetAllAdmins()
    {
        var admins = await _unitOfWork.Admins.GetAllAsync();
        
        var result = admins.Select(a => new AdminGetDto
        {
            Id = a.Id,
            Username = a.Username,
            Email = a.Email,
            Role = a.Role.ToString(),
            IsApproved = a.IsApproved,
            LastLogin = a.LastLogin,
            CreatedDate = a.CreatedDate
        });

        return Ok(result);
    }

    [HttpGet("analytics/activity-trends")]
    public async Task<IActionResult> GetActivityTrends([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? DateTime.Now.AddDays(-7);
        var end = endDate ?? DateTime.Now;

        // Fetch results within date range
        var results = await _unitOfWork.TestResults.FindAsync(tr => tr.TestDate >= start && tr.TestDate <= end);

        var grouped = results
            .GroupBy(tr => tr.TestDate.Date)
            .OrderBy(g => g.Key)
            .ToList();

        // Prepare labels and data
        var labels = grouped.Select(g => g.Key.ToString("yyyy-MM-dd")).ToArray();
        var data = grouped.Select(g => g.Count()).ToArray();

        return Ok(new
        {
            labels,
            datasets = new[]
            {
                new { label = "Games Played", data }
            }
        });
    }

    [HttpGet("analytics/difficult-questions")]
    public async Task<IActionResult> GetDifficultQuestions([FromQuery] int grade, [FromQuery] int subject, [FromQuery] int limit = 5)
    {
        var recentResults = await _unitOfWork.TestResults.FindAsync(
            tr => tr.TestDate >= DateTime.Now.AddDays(-30),
            tr => tr.Question!
        );

        var questionStats = recentResults
            .Where(tr => tr.Question != null)
            .Where(tr => (grade == 0 || (int)tr.Question!.GradeLevel == grade) && 
                         (subject == 0 || (int)tr.Question!.Subject == subject))
            .GroupBy(tr => tr.Question)
            .Select(g => new
            {
                Question = g.Key,
                TotalAttempts = g.Count(),
                ErrorRate = g.Count() > 0 ? (double)g.Count(tr => !tr.IsCorrect) / g.Count() * 100 : 0
            })
            .Where(x => x.TotalAttempts >= 1) 
            .OrderByDescending(x => x.ErrorRate)
            .ThenByDescending(x => x.TotalAttempts)
            .Take(limit)
            .Select(x => new
            {
                text = x.Question!.Text,
                errorRate = Math.Round(x.ErrorRate, 1),
                difficulty = x.Question!.DifficultyLevel.ToString(),
                attempts = x.TotalAttempts
            })
            .ToList();

        return Ok(questionStats);
    }

    [HttpGet("analytics/engagement-summary")]
    public async Task<IActionResult> GetEngagementSummary()
    {
        var allResults = await _unitOfWork.TestResults.GetAllAsync();
        
        var activeStudents = allResults.Select(r => r.StudentId).Distinct().Count();
        var completionRate = allResults.Any() ? (double)allResults.Count(r => r.Score >= 50) / allResults.Count() * 100 : 0;
        var averageScore = allResults.Any() ? allResults.Average(r => r.Score) : 0;

        return Ok(new
        {
            activeStudents,
            completionRate = Math.Round(completionRate, 1),
            averageScore = Math.Round(averageScore, 1)
        });
    }
}
