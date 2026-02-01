using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IWheelQuestionAttemptRepository : IGenericRepository<WheelQuestionAttempt>
{
    Task<IEnumerable<WheelQuestionAttempt>> GetSessionAttemptsAsync(long sessionId);
    Task<(int CorrectCount, int WrongCount)> GetQuestionStatsAsync(long questionId);
}

public class WheelQuestionAttemptRepository : GenericRepository<WheelQuestionAttempt>, IWheelQuestionAttemptRepository
{
    public WheelQuestionAttemptRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<WheelQuestionAttempt>> GetSessionAttemptsAsync(long sessionId)
    {
        return await _dbSet
            .Include(a => a.Question)
            .Where(a => a.SessionId == sessionId && !a.IsDeleted)
            .OrderBy(a => a.AttemptTime)
            .ToListAsync();
    }

    public async Task<(int CorrectCount, int WrongCount)> GetQuestionStatsAsync(long questionId)
    {
        var correct = await _dbSet.CountAsync(a => a.QuestionId == questionId && a.IsCorrect && !a.IsDeleted);
        var wrong = await _dbSet.CountAsync(a => a.QuestionId == questionId && !a.IsCorrect && !a.IsDeleted);
        return (correct, wrong);
    }
}
