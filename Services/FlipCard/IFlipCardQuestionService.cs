using System.Collections.Generic;
using System.Threading.Tasks;
using Nafes.API.DTOs.FlipCard;
using Nafes.API.Modules;
// using Nafes.API.DTOs.Shared; // For PaginationParams if needed

namespace Nafes.API.Services.FlipCard
{
    public interface IFlipCardQuestionService
    {
        Task<FlipCardQuestion> CreateQuestionAsync(CreateFlipCardQuestionDto dto);
        Task<FlipCardQuestion> UpdateQuestionAsync(int id, UpdateFlipCardQuestionDto dto);
        Task<IEnumerable<FlipCardQuestion>> GetByGradeAndSubjectAsync(int gradeId, int subjectId);
        Task<FlipCardQuestion?> GetByIdAsync(int id, bool includePairs = false);
        Task<bool> DeleteAsync(int id);
        Task<object> GetAllPaginatedAsync(int page, int pageSize); // Simplified return type
        Task<int> GetQuestionCountAsync(int gradeId, int subjectId);
        Task<IEnumerable<string>> GetCategoriesAsync(int? gradeId = null, int? subjectId = null);
        Task<FlipCardQuestion?> GetRandomQuestionAsync(int gradeId, int subjectId, int? difficultyLevel = null);
    }
}
