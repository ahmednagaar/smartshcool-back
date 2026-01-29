using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public class AdminRepository : GenericRepository<Admin>, IAdminRepository
{
    public AdminRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Admin?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.Username == username && !a.IsDeleted);
    }

    public async Task<Admin?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.Email == email && !a.IsDeleted);
    }
}
