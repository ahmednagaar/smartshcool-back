using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public class SystemSettingRepository : GenericRepository<SystemSetting>, ISystemSettingRepository
{
    public SystemSettingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<string?> GetValueAsync(string key)
    {
        var setting = await _dbSet.FirstOrDefaultAsync(s => s.Key == key && !s.IsDeleted);
        return setting?.Value;
    }

    public async Task<SystemSetting?> GetByKeyAsync(string key)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Key == key && !s.IsDeleted);
    }

    public async Task SetValueAsync(string key, string value)
    {
        var setting = await GetByKeyAsync(key);
        if (setting == null)
        {
            await _dbSet.AddAsync(new SystemSetting { Key = key, Value = value });
        }
        else
        {
            setting.Value = value;
            _dbSet.Update(setting);
        }
    }
}
