using Microsoft.AspNetCore.Mvc;
using Nafes.API.Modules;
using Nafes.API.Services;

namespace Nafes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpPost("visit")]
    public async Task<IActionResult> LogVisit([FromBody] VisitDTO visitDto)
    {
        var visit = new Visit
        {
            VisitorId = visitDto.VisitorId,
            PagePath = visitDto.PagePath,
            UserAgent = visitDto.UserAgent,
            DeviceType = visitDto.DeviceType,
            Source = visitDto.Source,
            StudentName = visitDto.StudentName,
            CreatedDate = DateTime.UtcNow
        };

        await _analyticsService.LogVisitAsync(visit);
        return Ok(new { message = "Visit logged" });
    }

    [HttpPost("event")]
    public async Task<IActionResult> LogEvent([FromBody] AnalyticsEventDTO eventDto)
    {
        var evt = new AnalyticsEvent
        {
            VisitorId = eventDto.VisitorId,
            EventName = eventDto.EventName,
            EventProperties = eventDto.EventProperties,
            PagePath = eventDto.PagePath,
            CreatedDate = DateTime.UtcNow
        };

        await _analyticsService.LogEventAsync(evt);
        return Ok(new { message = "Event logged" });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalVisits = await _analyticsService.GetTotalVisitsAsync();
        return Ok(new { totalVisits });
    }

    [HttpGet("visitors")]
    public async Task<IActionResult> GetRecentVisitors([FromQuery] int count = 20)
    {
        var visitors = await _analyticsService.GetRecentVisitorsAsync(count);
        return Ok(visitors);
    }

    [HttpGet("visitors/stats")]
    public async Task<IActionResult> GetVisitorStats()
    {
        var stats = await _analyticsService.GetVisitorStatsAsync();
        return Ok(stats);
    }
}

public class VisitDTO
{
    public string VisitorId { get; set; } = string.Empty;
    public string PagePath { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public string? DeviceType { get; set; }
    public string? Source { get; set; }
    public string? StudentName { get; set; }
}

public class AnalyticsEventDTO
{
    public string VisitorId { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string? EventProperties { get; set; }
    public string? PagePath { get; set; }
}
