using System.Threading.Tasks;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories.FlipCard
{
    public class FlipCardAttemptRepository : IFlipCardAttemptRepository
    {
        private readonly ApplicationDbContext _context;

        public FlipCardAttemptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FlipCardAttempt> CreateAsync(FlipCardAttempt attempt)
        {
            _context.FlipCardAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<FlipCardAttempt> GetByIdAsync(int id)
        {
            return await _context.FlipCardAttempts.FindAsync(id);
        }
    }
}
