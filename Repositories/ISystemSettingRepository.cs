using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface ISystemSettingRepository : IGenericRepository<SystemSetting>
{
    Task<string?> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
    Task<SystemSetting?> GetByKeyAsync(string key);
}
