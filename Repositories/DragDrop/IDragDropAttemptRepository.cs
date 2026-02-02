using System.Collections.Generic;
using System.Threading.Tasks;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IDragDropAttemptRepository
{
    Task<DragDropAttempt> CreateAttemptAsync(DragDropAttempt attempt);
    Task<IEnumerable<DragDropAttempt>> GetSessionAttemptsAsync(int sessionId);
    Task<Dictionary<int, double>> GetItemSuccessRatesAsync(int questionId); // For analytics
}
