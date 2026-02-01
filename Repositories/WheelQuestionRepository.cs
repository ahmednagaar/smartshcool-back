using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IWheelQuestionRepository : IGenericRepository<WheelQuestion>
{
    Task<IEnumerable<WheelQuestion>> GetByGradeAndSubjectAsync(GradeLevel grade, SubjectType subject, DifficultyLevel? difficulty = null);
    Task<IEnumerable<WheelQuestion>> GetRandomQuestionsAsync(GradeLevel grade, SubjectType subject, TestType testType, int count, DifficultyLevel? difficulty = null);
    Task<(IEnumerable<WheelQuestion> Items, int TotalCount)> SearchAsync(int page, int pageSize, GradeLevel? grade, SubjectType? subject, DifficultyLevel? difficulty, string? categoryTag, string? searchTerm);
    Task<int> GetQuestionCountByGradeSubjectAsync(GradeLevel grade, SubjectType subject);
    Task<IEnumerable<string>> GetCategoriesAsync(GradeLevel? grade, SubjectType? subject);
    Task BulkCreateAsync(IEnumerable<WheelQuestion> questions);
}

public class WheelQuestionRepository : GenericRepository<WheelQuestion>, IWheelQuestionRepository
{
    public WheelQuestionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<WheelQuestion>> GetByGradeAndSubjectAsync(GradeLevel grade, SubjectType subject, DifficultyLevel? difficulty = null)
    {
        var query = _dbSet.Where(q => q.GradeId == grade && q.SubjectId == subject && q.IsActive && !q.IsDeleted);
        
        if (difficulty.HasValue)
            query = query.Where(q => q.DifficultyLevel == difficulty.Value);

        return await query.OrderBy(q => q.DisplayOrder).ToListAsync();
    }

    public async Task<IEnumerable<WheelQuestion>> GetRandomQuestionsAsync(GradeLevel grade, SubjectType subject, TestType testType, int count, DifficultyLevel? difficulty = null)
    {
        var query = _dbSet.Where(q => q.GradeId == grade && q.SubjectId == subject && q.TestType == testType && q.IsActive && !q.IsDeleted);

        if (difficulty.HasValue)
            query = query.Where(q => q.DifficultyLevel == difficulty.Value);

        // Random ordering
        return await query.OrderBy(q => Guid.NewGuid()).Take(count).ToListAsync();
    }

    public async Task<(IEnumerable<WheelQuestion> Items, int TotalCount)> SearchAsync(
        int page, int pageSize, 
        GradeLevel? grade, SubjectType? subject, DifficultyLevel? difficulty, 
        string? categoryTag, string? searchTerm)
    {
        var query = _dbSet.AsQueryable();

        if (grade.HasValue) query = query.Where(q => q.GradeId == grade.Value);
        if (subject.HasValue) query = query.Where(q => q.SubjectId == subject.Value);
        if (difficulty.HasValue) query = query.Where(q => q.DifficultyLevel == difficulty.Value);
        if (!string.IsNullOrEmpty(categoryTag)) query = query.Where(q => q.CategoryTag == categoryTag);
        
        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(q => q.QuestionText.Contains(searchTerm) || q.CorrectAnswer.Contains(searchTerm));

        query = query.Where(q => !q.IsDeleted);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(q => q.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<int> GetQuestionCountByGradeSubjectAsync(GradeLevel grade, SubjectType subject)
    {
        return await _dbSet.CountAsync(q => q.GradeId == grade && q.SubjectId == subject && q.IsActive && !q.IsDeleted);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync(GradeLevel? grade, SubjectType? subject)
    {
        var query = _dbSet.AsQueryable();
        if (grade.HasValue) query = query.Where(q => q.GradeId == grade.Value);
        if (subject.HasValue) query = query.Where(q => q.SubjectId == subject.Value);

        return await query
            .Where(q => !string.IsNullOrEmpty(q.CategoryTag) && q.IsActive && !q.IsDeleted)
            .Select(q => q.CategoryTag!)
            .Distinct()
            .ToListAsync();
    }

    public async Task BulkCreateAsync(IEnumerable<WheelQuestion> questions)
    {
        await _dbSet.AddRangeAsync(questions);
        await _context.SaveChangesAsync();
    }
}
