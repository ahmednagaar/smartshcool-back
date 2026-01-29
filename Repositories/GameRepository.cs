using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public class GameRepository : GenericRepository<Game>, IGameRepository
{
    public GameRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Game?> GetWithQuestionsAsync(long id)
    {
        return await _dbSet
            .Include(g => g.GameQuestions)
                .ThenInclude(gq => gq.Question)
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted);
    }

    public async Task<IEnumerable<Game>> GetAllWithQuestionsAsync()
    {
        return await _dbSet
            .Include(g => g.GameQuestions)
                .ThenInclude(gq => gq.Question)
            .Where(g => !g.IsDeleted)
            .ToListAsync();
    }
}
