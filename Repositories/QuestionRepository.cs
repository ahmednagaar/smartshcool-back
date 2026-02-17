using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;
using Microsoft.IdentityModel.Tokens;

using Nafes.API.DTOs.Question;

namespace Nafes.API.Repositories;

public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
{
    public QuestionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Question>> GetByDifficultyAsync(DifficultyLevel difficulty)
    {
        return await _dbSet
            .Where(q => q.Difficulty == difficulty && !q.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetByTypeAsync(QuestionType type)
    {
        return await _dbSet
            .Where(q => q.Type == type && !q.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetFilteredAsync(GradeLevel grade, SubjectType subject, TestType testType)
    {
        return await _dbSet
            .Include(q => q.SubQuestions.OrderBy(sq => sq.OrderIndex))
            .Where(q => q.Grade == grade && q.Subject == subject && q.TestType == testType && !q.IsDeleted)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Question> Items, int TotalCount)> SearchAsync(QuestionSearchRequestDto searchDto)
    {
        var query = _dbSet.Include(q => q.SubQuestions.OrderBy(sq => sq.OrderIndex)).AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            query = query.Where(q => q.Text.Contains(searchDto.SearchTerm));
        }

        if (searchDto.Grade.HasValue)
        {
            query = query.Where(q => q.Grade == searchDto.Grade.Value);
        }

        if (searchDto.Subject.HasValue)
        {
            query = query.Where(q => q.Subject == searchDto.Subject.Value);
        }

        if (searchDto.Difficulty.HasValue)
        {
            query = query.Where(q => q.Difficulty == searchDto.Difficulty.Value);
        }

        if (searchDto.Type.HasValue)
        {
            query = query.Where(q => q.Type == searchDto.Type.Value);
        }

        if (searchDto.TestType.HasValue)
        {
            query = query.Where(q => q.TestType == searchDto.TestType.Value);
        }

        query = query.Where(q => !q.IsDeleted);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = searchDto.SortBy?.ToLower() switch
        {
            "text" => searchDto.SortOrder == "asc" ? query.OrderBy(q => q.Text) : query.OrderByDescending(q => q.Text),
            "grade" => searchDto.SortOrder == "asc" ? query.OrderBy(q => q.Grade) : query.OrderByDescending(q => q.Grade),
            "subject" => searchDto.SortOrder == "asc" ? query.OrderBy(q => q.Subject) : query.OrderByDescending(q => q.Subject),
            "type" => searchDto.SortOrder == "asc" ? query.OrderBy(q => q.Type) : query.OrderByDescending(q => q.Type),
            "difficulty" => searchDto.SortOrder == "asc" ? query.OrderBy(q => q.Difficulty) : query.OrderByDescending(q => q.Difficulty),
            "createddate" => searchDto.SortOrder == "asc" ? query.OrderBy(q => q.CreatedDate) : query.OrderByDescending(q => q.CreatedDate),
            _ => query.OrderByDescending(q => q.CreatedDate)
        };

        // Apply pagination
        var items = await query
            //.OrderByDescending(q => q.CreatedDate) // Removed as we handle sorting above
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Question>> GetAvailableForGameAsync(long gameId, GradeLevel grade, SubjectType subject, string? searchTerm = null)
    {
        var query = _dbSet.AsQueryable();

        // Filter by Grade and Subject
        query = query.Where(q => q.Grade == grade && q.Subject == subject && !q.IsDeleted);

        // Exclude questions already in the game
        query = query.Where(q => !q.GameQuestions.Any(gq => gq.GameId == gameId));

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(q => q.Text.Contains(searchTerm));
        }

        return await query.ToListAsync();
    }

    public async Task<Question?> GetIncludeDeletedAsync(long id)
    {
        return await _dbSet.FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<Question?> GetByIdWithSubQuestionsAsync(long id)
    {
        return await _dbSet
            .Include(q => q.SubQuestions.OrderBy(sq => sq.OrderIndex))
            .FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted);
    }
}

