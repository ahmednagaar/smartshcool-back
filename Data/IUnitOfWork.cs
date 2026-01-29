using Microsoft.EntityFrameworkCore.Storage;
using Nafes.API.Repositories;

namespace Nafes.API.Data;

public interface IUnitOfWork : IDisposable
{
    // Repositories will be added here
    IStudentRepository Students { get; }
    IQuestionRepository Questions { get; }
    IGameRepository Games { get; }
    ITestResultRepository TestResults { get; }
    IAdminRepository Admins { get; }
    IAuditLogRepository AuditLogs { get; }
    ISystemSettingRepository SystemSettings { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> CommitAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
