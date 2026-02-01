using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IMatchingQuestionRepository : IGenericRepository<MatchingQuestion>
{
    Task<IEnumerable<MatchingQuestion>> GetByGradeAndSubjectAsync(GradeLevel grade, SubjectType subject);
    Task<(IEnumerable<MatchingQuestion> Items, int TotalCount)> SearchAsync(int page, int pageSize, GradeLevel? grade, SubjectType? subject, string? searchTerm);
    Task BulkCreateAsync(IEnumerable<MatchingQuestion> questions);
}

public class MatchingQuestionRepository : GenericRepository<MatchingQuestion>, IMatchingQuestionRepository
{
    public MatchingQuestionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MatchingQuestion>> GetByGradeAndSubjectAsync(GradeLevel grade, SubjectType subject)
    {
        return await _dbSet
            .Where(q => q.GradeId == grade && q.SubjectId == subject && q.IsActive && !q.IsDeleted)
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync();
    }

    public async Task<(IEnumerable<MatchingQuestion> Items, int TotalCount)> SearchAsync(int page, int pageSize, GradeLevel? grade, SubjectType? subject, string? searchTerm)
    {
        var query = _dbSet.AsQueryable();

        // Filters
        if (grade.HasValue) 
            query = query.Where(q => q.GradeId == grade.Value);
            
        if (subject.HasValue) 
            query = query.Where(q => q.SubjectId == subject.Value);

        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(q => q.LeftItemText.Contains(searchTerm) || q.RightItemText.Contains(searchTerm));

        query = query.Where(q => !q.IsDeleted);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(q => q.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task BulkCreateAsync(IEnumerable<MatchingQuestion> questions)
    {
        await _dbSet.AddRangeAsync(questions);
        await _context.SaveChangesAsync();
    }
}
