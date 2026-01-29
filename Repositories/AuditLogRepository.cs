using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task LogAsync(string action, string entityName, string entityId, string details, long? adminId, string adminUsername)
    {
        var log = new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Details = details,
            AdminId = adminId,
            AdminUsername = adminUsername,
            CreatedDate = DateTime.UtcNow
        };
        
        await _dbSet.AddAsync(log);
    }
    
    public async Task<IEnumerable<AuditLog>> GetLatestLogsAsync(int count)
    {
        return await _dbSet
            .OrderByDescending(x => x.CreatedDate)
            .Take(count)
            .ToListAsync();
    }
}
