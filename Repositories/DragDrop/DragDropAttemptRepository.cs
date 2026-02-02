using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nafes.API.Repositories;

public class DragDropAttemptRepository : IDragDropAttemptRepository
{
    private readonly ApplicationDbContext _context;

    public DragDropAttemptRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DragDropAttempt> CreateAttemptAsync(DragDropAttempt attempt)
    {
        _context.DragDropAttempts.Add(attempt);
        await _context.SaveChangesAsync();
        return attempt;
    }

    public async Task<IEnumerable<DragDropAttempt>> GetSessionAttemptsAsync(int sessionId)
    {
        return await _context.DragDropAttempts
            .Include(a => a.Item)
            .Include(a => a.PlacedInZone)
            .Where(a => a.SessionId == sessionId)
            .OrderBy(a => a.AttemptTime)
            .ToListAsync();
    }

    public async Task<Dictionary<int, double>> GetItemSuccessRatesAsync(int questionId)
    {
        // This might be expensive for large datasets
        var attempts = await _context.DragDropAttempts
            .Include(a => a.Item)
            .Where(a => a.Item.DragDropQuestionId == questionId)
            .GroupBy(a => a.ItemId)
            .Select(g => new
            {
                ItemId = g.Key,
                SuccessRate = (double)g.Count(a => a.IsCorrect) / g.Count() * 100
            })
            .ToListAsync();

        return attempts.ToDictionary(k => k.ItemId, v => v.SuccessRate);
    }
}
