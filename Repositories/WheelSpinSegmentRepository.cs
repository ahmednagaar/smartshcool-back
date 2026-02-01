using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IWheelSpinSegmentRepository : IGenericRepository<WheelSpinSegment>
{
    Task<IEnumerable<WheelSpinSegment>> GetActiveSegmentsAsync();
}

public class WheelSpinSegmentRepository : GenericRepository<WheelSpinSegment>, IWheelSpinSegmentRepository
{
    public WheelSpinSegmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<WheelSpinSegment>> GetActiveSegmentsAsync()
    {
        // Cache this? For now DB call is fine given low traffic
        return await _dbSet
            .Where(s => s.IsActive && !s.IsDeleted)
            .OrderByDescending(s => s.Probability) // Determine logic? Or random
            .ToListAsync();
    }
}
