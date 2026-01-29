using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Nafes.API.Repositories;
using Nafes.API.Modules;
using Nafes.API.Data;

namespace Nafes.API.Controllers;

[Route("api/audit")]
[ApiController]
[Authorize(Roles = "SuperAdmin")]
public class AuditLogController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditLogController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("logs")]
    public async Task<ActionResult<IEnumerable<AuditLog>>> GetLatestLogs([FromQuery] int count = 100)
    {
        var logs = await _unitOfWork.AuditLogs.GetLatestLogsAsync(count);
        return Ok(logs);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportLogs()
    {
        var logs = await _unitOfWork.AuditLogs.GetLatestLogsAsync(1000); // Limit to last 1000 logs
        var builder = new StringBuilder();
        
        builder.Append('\uFEFF');
        builder.AppendLine("التاريخ,المسؤول,الإجراء,الكيان,التفاصيل");
        
        foreach (var l in logs)
        {
            builder.AppendLine($"{l.CreatedDate},{EscapeCsv(l.AdminUsername)},{l.Action},{l.EntityName} #{l.EntityId},{EscapeCsv(l.Details)}");
        }

        return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"audit_logs_export_{DateTime.Now:yyyyMMdd}.csv");
    }

    private string EscapeCsv(string field)
    {
        if (string.IsNullOrEmpty(field)) return "";
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}
