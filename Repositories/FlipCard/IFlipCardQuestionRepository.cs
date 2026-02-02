using System.Collections.Generic;
using System.Threading.Tasks;
using Nafes.API.DTOs.Shared;
using Nafes.API.Modules;

namespace Nafes.API.Repositories.FlipCard
{
    public interface IFlipCardQuestionRepository
    {
        Task<IEnumerable<FlipCardQuestion>> GetByGradeAndSubjectAsync(int gradeId, int subjectId);
        Task<FlipCardQuestion?> GetByIdAsync(int id, bool includePairs = false);
        Task<FlipCardQuestion> CreateAsync(FlipCardQuestion question);
        Task<FlipCardQuestion> UpdateAsync(FlipCardQuestion question);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<FlipCardQuestion>> GetAllPaginatedAsync(int page, int pageSize); // Simplified pagination for now
        Task<int> GetQuestionCountAsync(int gradeId, int subjectId);
        Task<FlipCardQuestion?> GetRandomQuestionAsync(int gradeId, int subjectId, int? difficultyLevel = null);
        Task<IEnumerable<string>> GetCategoriesAsync(int? gradeId = null, int? subjectId = null);
    }
}
