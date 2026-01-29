using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IGameRepository : IGenericRepository<Game>
{
    Task<Game?> GetWithQuestionsAsync(long id);
    Task<IEnumerable<Game>> GetAllWithQuestionsAsync();
}
