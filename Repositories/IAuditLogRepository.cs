using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IAuditLogRepository : IGenericRepository<AuditLog>
{
    Task LogAsync(string action, string entityName, string entityId, string details, long? adminId, string adminUsername);
    Task<IEnumerable<AuditLog>> GetLatestLogsAsync(int count);
}
