using Microsoft.EntityFrameworkCore.Storage;
using Nafes.API.Repositories;

namespace Nafes.API.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public IStudentRepository Students { get; }
    public IQuestionRepository Questions { get; }
    public IGameRepository Games { get; }
    public ITestResultRepository TestResults { get; }
    public IAdminRepository Admins { get; }
    public IAuditLogRepository AuditLogs { get; }
    public ISystemSettingRepository SystemSettings { get; }
    public IRefreshTokenRepository RefreshTokens { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IStudentRepository students,
        IQuestionRepository questions,
        IGameRepository games,
        ITestResultRepository testResults,
        IAdminRepository admins,
        IAuditLogRepository auditLogs,
        ISystemSettingRepository systemSettings,
        IRefreshTokenRepository refreshTokens)
    {
        _context = context;
        Students = students;
        Questions = questions;
        Games = games;
        TestResults = testResults;
        Admins = admins;
        AuditLogs = auditLogs;
        SystemSettings = systemSettings;
        RefreshTokens = refreshTokens;
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction;
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
