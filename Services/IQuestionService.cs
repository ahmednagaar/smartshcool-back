using Nafes.API.DTOs.Question;
using Nafes.API.DTOs.Shared;

namespace Nafes.API.Services;

public interface IQuestionService
{
    Task<PaginatedResponse<QuestionGetDto>> GetQuestionsAsync(QuestionSearchRequestDto request);
    Task<ApiResponse<QuestionGetDto>> GetQuestionByIdAsync(long id);
    Task<ApiResponse<QuestionGetDto>> CreateQuestionAsync(QuestionCreateDto createDto);
    Task<ApiResponse<QuestionGetDto>> UpdateQuestionAsync(long id, QuestionUpdateDto updateDto);
    Task<ApiResponse<bool>> DeleteQuestionAsync(long id);
    Task<ApiResponse<BulkImportResultDto>> BulkImportQuestionsAsync(List<QuestionCreateDto> questions);
    Task<ApiResponse<byte[]>> ExportQuestionsAsync(string format, QuestionSearchRequestDto request);
    Task<ApiResponse<object>> GetQuestionStatsAsync();
}
