using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbSet
            .Include(rt => rt.Admin)
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsDeleted);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByAdminIdAsync(long adminId)
    {
        return await _dbSet
            .Where(rt => rt.AdminId == adminId && 
                        !rt.IsRevoked && 
                        rt.ExpiryDate > DateTime.UtcNow &&
                        !rt.IsDeleted)
            .ToListAsync();
    }
}
