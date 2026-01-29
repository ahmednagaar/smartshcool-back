using Nafes.API.Modules;

namespace Nafes.API.Services;

public interface IAnalyticsService
{
    Task LogVisitAsync(Visit visit);
    Task LogEventAsync(AnalyticsEvent analyticsEvent);
    Task<int> GetTotalVisitsAsync();
}
