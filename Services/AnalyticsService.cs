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
        // Count distinct VisitorIds or total visits? 
        // For visitor counter badge, we usually want distinct or total unique visits.
        // Let's return total unique visits for now.
        return await _context.Visits.Select(v => v.VisitorId).Distinct().CountAsync();
    }
}
