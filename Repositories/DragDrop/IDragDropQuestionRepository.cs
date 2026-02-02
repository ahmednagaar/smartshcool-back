using System.Collections.Generic;
using System.Threading.Tasks;
using Nafes.API.DTOs.Shared;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IDragDropQuestionRepository
{
    Task<IEnumerable<DragDropQuestion>> GetByGradeAndSubjectAsync(GradeLevel grade, SubjectType subject);
    Task<DragDropQuestion?> GetByIdAsync(int id, bool includeZonesAndItems = false);
    Task<DragDropQuestion> CreateAsync(DragDropQuestion question);
    Task<DragDropQuestion> UpdateAsync(DragDropQuestion question);
    Task<bool> DeleteAsync(int id); // Soft delete
    Task<PaginatedResult<DragDropQuestion>> GetAllPaginatedAsync(PaginationParams paginationParams, GradeLevel? grade = null, SubjectType? subject = null, DifficultyLevel? difficulty = null);
    Task<int> GetQuestionCountAsync(GradeLevel grade, SubjectType subject);
    Task<DragDropQuestion?> GetRandomQuestionAsync(GradeLevel grade, SubjectType subject, DifficultyLevel? difficulty = null);
}
