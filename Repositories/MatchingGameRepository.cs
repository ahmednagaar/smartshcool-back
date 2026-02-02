using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs.Shared;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public class MatchingGameRepository : IMatchingGameRepository
{
    private readonly ApplicationDbContext _context;

    public MatchingGameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MatchingGame?> GetByIdAsync(long id, bool includePairs = false)
    {
        var query = _context.MatchingGames.AsQueryable();
        if (includePairs)
        {
            query = query.Include(g => g.Pairs);
        }
        return await query.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<MatchingGame>> GetAllAsync()
    {
        return await _context.MatchingGames
            .Include(g => g.Pairs)
            .OrderByDescending(g => g.CreatedDate)
            .ToListAsync();
    }

    public async Task<PaginatedResult<MatchingGame>> GetAvailableGamesAsync(int page, int pageSize, GradeLevel? grade = null, SubjectType? subject = null)
    {
        var query = _context.MatchingGames.Where(g => g.IsActive);

        if (grade.HasValue)
            query = query.Where(g => g.GradeId == grade.Value);
            
        if (subject.HasValue)
            query = query.Where(g => g.SubjectId == subject.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(g => g.DisplayOrder)
            .ThenByDescending(g => g.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<MatchingGame>
        {
            Items = items,
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<MatchingGame> CreateAsync(MatchingGame game)
    {
        game.CreatedDate = DateTime.UtcNow;
        _context.MatchingGames.Add(game);
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task<MatchingGame> UpdateAsync(MatchingGame game)
    {
        _context.Entry(game).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var game = await _context.MatchingGames.FindAsync(id);
        if (game == null) return false;

        _context.MatchingGames.Remove(game);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<MatchingGame?> GetRandomGameAsync(GradeLevel grade, SubjectType subject, DifficultyLevel? difficulty = null)
    {
        var query = _context.MatchingGames
            .Include(g => g.Pairs)
            .Where(g => g.IsActive && g.GradeId == grade && g.SubjectId == subject);

        if (difficulty.HasValue)
            query = query.Where(g => g.DifficultyLevel == difficulty.Value);

        var games = await query.ToListAsync();
        
        if (!games.Any()) return null;
        
        var random = new Random();
        return games[random.Next(games.Count)];
    }
}
