using Nafes.API.Modules;

namespace Nafes.API.Services;

public class VisitorStatsDto
{
    public int WithName { get; set; }
    public int WithoutName { get; set; }
    public int Total { get; set; }
}

public class RecentVisitorDto
{
    public string Name { get; set; } = string.Empty;
    public string? DeviceType { get; set; }
    public DateTime VisitedAt { get; set; }
    public string VisitorId { get; set; } = string.Empty;
    public int TotalVisits { get; set; }
}

public interface IAnalyticsService
{
    Task LogVisitAsync(Visit visit);
    Task LogEventAsync(AnalyticsEvent analyticsEvent);
    Task<int> GetTotalVisitsAsync();
    Task<List<RecentVisitorDto>> GetRecentVisitorsAsync(int count = 20);
    Task<VisitorStatsDto> GetVisitorStatsAsync();
}
