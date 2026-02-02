using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.DTOs.Shared;
using Nafes.API.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nafes.API.Repositories;

public class DragDropQuestionRepository : IDragDropQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public DragDropQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DragDropQuestion>> GetByGradeAndSubjectAsync(GradeLevel grade, SubjectType subject)
    {
        return await _context.DragDropQuestions
            .Include(q => q.Zones)
            .Include(q => q.Items)
            .Where(q => q.Grade == grade && q.Subject == subject && q.IsActive)
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync();
    }

    public async Task<DragDropQuestion?> GetByIdAsync(int id, bool includeZonesAndItems = false)
    {
        var query = _context.DragDropQuestions.AsQueryable();

        if (includeZonesAndItems)
        {
            query = query.Include(q => q.Zones.OrderBy(z => z.ZoneOrder))
                         .Include(q => q.Items.OrderBy(i => i.ItemOrder));
        }

        return await query.FirstOrDefaultAsync(q => q.Id == id && q.IsActive);
    }

    public async Task<DragDropQuestion> CreateAsync(DragDropQuestion question)
    {
        _context.DragDropQuestions.Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<DragDropQuestion> UpdateAsync(DragDropQuestion question)
    {
        question.LastModifiedDate = DateTime.UtcNow;
        _context.DragDropQuestions.Update(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var question = await GetByIdAsync(id);
        if (question == null) return false;

        question.IsActive = false; // Soft delete
        question.LastModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PaginatedResult<DragDropQuestion>> GetAllPaginatedAsync(PaginationParams paginationParams, GradeLevel? grade = null, SubjectType? subject = null, DifficultyLevel? difficulty = null)
    {
        var query = _context.DragDropQuestions.AsQueryable();

        // Ensure we only get active questions (unless backend for admin needs to see deleted? Prompt says soft delete, usually lists active)
        query = query.Where(q => q.IsActive);

        if (grade.HasValue)
            query = query.Where(q => q.Grade == grade.Value);

        if (subject.HasValue)
            query = query.Where(q => q.Subject == subject.Value);

        if (difficulty.HasValue)
            query = query.Where(q => q.DifficultyLevel == difficulty.Value);

        if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
            query = query.Where(q => q.GameTitle.Contains(paginationParams.SearchTerm) || (q.Instructions != null && q.Instructions.Contains(paginationParams.SearchTerm)));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(q => q.CreatedDate)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return new PaginatedResult<DragDropQuestion>(items, totalCount, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<int> GetQuestionCountAsync(GradeLevel grade, SubjectType subject)
    {
        return await _context.DragDropQuestions
            .CountAsync(q => q.Grade == grade && q.Subject == subject && q.IsActive);
    }

    public async Task<DragDropQuestion?> GetRandomQuestionAsync(GradeLevel grade, SubjectType subject, DifficultyLevel? difficulty = null)
    {
        var query = _context.DragDropQuestions
            .AsNoTracking() // Performance
            .Include(q => q.Zones)
            .Include(q => q.Items)
            .Where(q => q.Grade == grade && q.Subject == subject && q.IsActive);

        if (difficulty.HasValue)
        {
            query = query.Where(q => q.DifficultyLevel == difficulty.Value);
        }

        var count = await query.CountAsync();
        if (count == 0) return null;

        var index = new Random().Next(count);
        return await query.Skip(index).FirstOrDefaultAsync();
    }
}
