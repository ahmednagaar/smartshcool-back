using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _context;

    public AnalyticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogVisitAsync(Visit visit)
    {
        await _context.Visits.AddAsync(visit);
        await _context.SaveChangesAsync();
    }

    public async Task LogEventAsync(AnalyticsEvent analyticsEvent)
    {
        await _context.AnalyticsEvents.AddAsync(analyticsEvent);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTotalVisitsAsync()
    {
        return await _context.Visits.Select(v => v.VisitorId).Distinct().CountAsync();
    }

    public async Task<List<RecentVisitorDto>> GetRecentVisitorsAsync(int count = 20)
    {
        // Get the most recent visit per named visitor, grouped by VisitorId
        var visitors = await _context.Visits
            .Where(v => v.StudentName != null && v.StudentName != "")
            .GroupBy(v => v.VisitorId)
            .Select(g => new RecentVisitorDto
            {
                VisitorId = g.Key,
                Name = g.OrderByDescending(v => v.CreatedDate).First().StudentName!,
                DeviceType = g.OrderByDescending(v => v.CreatedDate).First().DeviceType,
                VisitedAt = g.Max(v => v.CreatedDate),
                TotalVisits = g.Count()
            })
            .OrderByDescending(v => v.VisitedAt)
            .Take(count)
            .ToListAsync();

        return visitors;
    }

    public async Task<VisitorStatsDto> GetVisitorStatsAsync()
    {
        // Get distinct visitorIds with and without names
        var allVisitorIds = await _context.Visits
            .Select(v => new { v.VisitorId, v.StudentName })
            .ToListAsync();

        var grouped = allVisitorIds
            .GroupBy(v => v.VisitorId)
            .Select(g => new
            {
                VisitorId = g.Key,
                HasName = g.Any(v => !string.IsNullOrEmpty(v.StudentName))
            })
            .ToList();

        var withName = grouped.Count(g => g.HasName);
        var withoutName = grouped.Count(g => !g.HasName);

        return new VisitorStatsDto
        {
            WithName = withName,
            WithoutName = withoutName,
            Total = withName + withoutName
        };
    }
}
