using System.Collections.Generic;
using System.Threading.Tasks;
using Nafes.API.DTOs.DragDrop;
using Nafes.API.DTOs.Shared;
using Nafes.API.Modules;

namespace Nafes.API.Services;

public interface IDragDropQuestionService
{
    Task<DragDropQuestionDto> CreateQuestionAsync(CreateDragDropQuestionDto dto, int userId);
    Task<DragDropQuestionDto> UpdateQuestionAsync(UpdateDragDropQuestionDto dto, int userId);
    Task<DragDropQuestionDto?> GetQuestionByIdAsync(int id);
    Task<PaginatedResult<DragDropQuestionDto>> GetAllPaginatedAsync(PaginationParams paginationParams, GradeLevel? grade = null, SubjectType? subject = null);
    Task<bool> DeleteQuestionAsync(int id);
    Task<IEnumerable<DragDropQuestionDto>> GetQuestionsByGradeAndSubjectAsync(GradeLevel grade, SubjectType subject);
}
