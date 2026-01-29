using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IAdminRepository : IGenericRepository<Admin>
{
    Task<Admin?> GetByUsernameAsync(string username);
    Task<Admin?> GetByEmailAsync(string email);
}
