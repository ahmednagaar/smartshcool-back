using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.Repositories;
using Nafes.API.Modules;
using Nafes.API.Data;

namespace Nafes.API.Controllers;

[Route("api/settings")]
[ApiController]
[Authorize(Roles = "SuperAdmin")]
public class SystemSettingController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public SystemSettingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SystemSetting>>> GetSettings()
    {
        var settings = await _unitOfWork.SystemSettings.GetAllAsync();
        return Ok(settings);
    }

    [HttpPut("{key}")]
    public async Task<ActionResult> UpdateSetting(string key, [FromBody] SystemSettingDto dto)
    {
        var setting = await _unitOfWork.SystemSettings.GetByKeyAsync(key);
        if (setting == null)
        {
            // Auto create if not exists? Or 404.
            // Let's auto-create for flexibility
             setting = new SystemSetting
             {
                 Key = key,
                 Value = dto.Value,
                 Description = dto.Description ?? "",
                 Type = dto.Type ?? "string",
                 Group = dto.Group ?? "General"
             };
             await _unitOfWork.SystemSettings.AddAsync(setting);
             
             // Log
             await LogAction("Create", key, $"Created setting {key} = {dto.Value}");
        }
        else
        {
            var oldValue = setting.Value;
            setting.Value = dto.Value;
            if(!string.IsNullOrEmpty(dto.Description)) setting.Description = dto.Description;
            
            _unitOfWork.SystemSettings.Update(setting);
            
            // Log
            await LogAction("Update", key, $"Updated setting {key} from '{oldValue}' to '{dto.Value}'");
        }

        await _unitOfWork.CommitAsync();
        return Ok(new { message = "تم تحديث الإعداد بنجاح", setting });
    }

    private async Task LogAction(string action, string key, string details)
    {
        var adminId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
        await _unitOfWork.AuditLogs.LogAsync(action, "SystemSetting", key, details, adminId, adminName);
    }
}

public class SystemSettingDto
{
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Group { get; set; }
    public string? Type { get; set; }
}
