using AutoMapper;
using Nafes.API.DTOs.Question;
using Nafes.API.DTOs.Shared;
using Nafes.API.Modules;
using Nafes.API.Repositories;

namespace Nafes.API.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;
    private readonly ISanitizationService _sanitizer;
    private readonly ILogger<QuestionService> _logger;

    public QuestionService(
        IQuestionRepository questionRepository,
        IMapper mapper,
        ISanitizationService sanitizer,
        ILogger<QuestionService> logger)
    {
        _questionRepository = questionRepository;
        _mapper = mapper;
        _sanitizer = sanitizer;
        _logger = logger;
    }

    public async Task<PaginatedResponse<QuestionGetDto>> GetQuestionsAsync(QuestionSearchRequestDto request)
    {
        try
        {
            var (items, totalCount) = await _questionRepository.SearchAsync(request);
            var dtos = _mapper.Map<IEnumerable<QuestionGetDto>>(items);
            
            return PaginatedResponse<QuestionGetDto>.Ok(
                dtos, 
                request.Page, 
                request.PageSize, 
                totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving questions");
            return new PaginatedResponse<QuestionGetDto>
            {
                Success = false,
                Message = "حدث خطأ أثناء استرجاع الأسئلة",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<QuestionGetDto>> GetQuestionByIdAsync(long id)
    {
        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
        {
            return ApiResponse<QuestionGetDto>.Error("السؤال غير موجود");
        }
        
        return ApiResponse<QuestionGetDto>.Ok(_mapper.Map<QuestionGetDto>(question));
    }

    public async Task<ApiResponse<QuestionGetDto>> CreateQuestionAsync(QuestionCreateDto createDto)
    {
        try
        {
            // Sanitize input
            createDto.Text = _sanitizer.Sanitize(createDto.Text);
            
            if (!string.IsNullOrEmpty(createDto.MediaUrl) && !_sanitizer.IsValidMediaUrl(createDto.MediaUrl))
            {
                return ApiResponse<QuestionGetDto>.Error("رابط الوسائط غير صالح");
            }

            var question = _mapper.Map<Question>(createDto);
            question.CreatedDate = DateTime.UtcNow;
            
            await _questionRepository.AddAsync(question);
            await _questionRepository.SaveChangesAsync();

            var resultDto = _mapper.Map<QuestionGetDto>(question);
            return ApiResponse<QuestionGetDto>.Ok(resultDto, "تم إضافة السؤال بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            return ApiResponse<QuestionGetDto>.Error("فشل في إضافة السؤال", ex);
        }
    }

    public async Task<ApiResponse<QuestionGetDto>> UpdateQuestionAsync(long id, QuestionUpdateDto updateDto)
    {
        try
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
            {
                return ApiResponse<QuestionGetDto>.Error("السؤال غير موجود");
            }

            // Sanitize
            if (!string.IsNullOrEmpty(updateDto.Text))
            {
                updateDto.Text = _sanitizer.Sanitize(updateDto.Text);
            }

            if (!string.IsNullOrEmpty(updateDto.MediaUrl) && !_sanitizer.IsValidMediaUrl(updateDto.MediaUrl))
            {
                return ApiResponse<QuestionGetDto>.Error("رابط الوسائط غير صالح");
            }

            _mapper.Map(updateDto, question);
            _questionRepository.Update(question);
            await _questionRepository.SaveChangesAsync();

            return ApiResponse<QuestionGetDto>.Ok(_mapper.Map<QuestionGetDto>(question), "تم تحديث السؤال بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question {Id}", id);
            return ApiResponse<QuestionGetDto>.Error("فشل في تحديث السؤال", ex);
        }
    }

    public async Task<ApiResponse<bool>> DeleteQuestionAsync(long id)
    {
        try
        {
            var question = await _questionRepository.GetIncludeDeletedAsync(id);
            if (question == null)
            {
                return ApiResponse<bool>.Error("السؤال غير موجود");
            }

            // Soft delete
            question.IsDeleted = true;
            _questionRepository.Update(question);
            await _questionRepository.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "تم حذف السؤال بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question {Id}", id);
            return ApiResponse<bool>.Error("فشل في حذف السؤال", ex);
        }
    }

    public async Task<ApiResponse<BulkImportResultDto>> BulkImportQuestionsAsync(List<QuestionCreateDto> questions)
    {
        var result = new BulkImportResultDto();
        int currentRow = 0;

        foreach (var dto in questions)
        {
            currentRow++;
            try
            {
                // Basic validation
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
                
                if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(dto, context, validationResults, true))
                {
                    result.Failures.Add(new ImportFailure
                    {
                        Row = currentRow,
                        Errors = validationResults.Select(r => r.ErrorMessage ?? "Validation Error").ToList()
                    });
                    result.FailureCount++;
                    continue;
                }

                var createResponse = await CreateQuestionAsync(dto);
                if (createResponse.Success)
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.Failures.Add(new ImportFailure
                    {
                        Row = currentRow,
                        Errors = createResponse.Errors
                    });
                    result.FailureCount++;
                }
            }
            catch (Exception ex)
            {
                result.Failures.Add(new ImportFailure
                {
                    Row = currentRow,
                    Errors = new List<string> { ex.Message }
                });
                result.FailureCount++;
            }
        }

        string message = $"تم استيراد {result.SuccessCount} سؤال بنجاح. فشل {result.FailureCount}.";
        return ApiResponse<BulkImportResultDto>.Ok(result, message);
    }

    public async Task<ApiResponse<byte[]>> ExportQuestionsAsync(string format, QuestionSearchRequestDto request)
    {
        // For export, we might want to fetch all matching results, not just one page
        // So we might set a very large page size
        request.Page = 1;
        request.PageSize = 10000; // Cap at 10k for safety

        var (items, _) = await _questionRepository.SearchAsync(request);
        
        // Note: The actual file generation logic (CSV/JSON/PDF) is often better handled 
        // by a dedicated file service or library. For now, we'll return null and handle simpler logic.
        // Or strictly speaking, the controller could handle serialization if we just return the data.
        // But if we want to return byte[], we should generate it here.
        
        // This is a placeholder for server-side export. 
        // Since we implemented client-side export in the frontend, this might be redundant 
        // but good for large datasets.
        
        throw new NotImplementedException("Server-side export is not fully implemented yet. Use client-side export.");
    }

    public async Task<ApiResponse<object>> GetQuestionStatsAsync()
    {
        try
        {
            // This would ideally use a more optimized query
            var total = await _questionRepository.CountAsync();
            return ApiResponse<object>.Ok(new { TotalCount = total });
        }
        catch (Exception ex)
        {
             return ApiResponse<object>.Error("Failed to get stats", ex);
        }
    }
}
