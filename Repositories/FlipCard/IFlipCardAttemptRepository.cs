using System.Threading.Tasks;
using Nafes.API.Modules;

namespace Nafes.API.Repositories.FlipCard
{
    public interface IFlipCardAttemptRepository
    {
        Task<FlipCardAttempt> CreateAsync(FlipCardAttempt attempt);
        Task<FlipCardAttempt> GetByIdAsync(int id);
    }
}
