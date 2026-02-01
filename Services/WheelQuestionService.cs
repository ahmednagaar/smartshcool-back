using AutoMapper;
using Nafes.API.DTOs.Shared;
using Nafes.API.DTOs.WheelGame;
using Nafes.API.Modules;
using Nafes.API.Repositories;
using System.Security.Claims;

namespace Nafes.API.Services;

public interface IWheelQuestionService
{
    Task<(IEnumerable<WheelQuestionResponseDto> Items, int TotalCount)> SearchAsync(int page, int pageSize, GradeLevel? grade, SubjectType? subject, DifficultyLevel? difficulty, string? categoryTag, string? searchTerm);
    Task<WheelQuestionResponseDto> GetByIdAsync(long id);
    Task<WheelQuestionResponseDto> CreateAsync(CreateWheelQuestionDto dto, string userId);
    Task<WheelQuestionResponseDto> UpdateAsync(long id, UpdateWheelQuestionDto dto, string userId);
    Task DeleteAsync(long id, string userId);
    Task BulkImportAsync(List<CreateWheelQuestionDto> dtos, string userId);
    Task<int> GetQuestionCountAsync(GradeLevel grade, SubjectType subject);
    Task<IEnumerable<string>> GetCategoriesAsync(GradeLevel? grade, SubjectType? subject);
}

public class WheelQuestionService : IWheelQuestionService
{
    private readonly IWheelQuestionRepository _repository;
    private readonly IMapper _mapper;

    public WheelQuestionService(IWheelQuestionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<WheelQuestionResponseDto> Items, int TotalCount)> SearchAsync(int page, int pageSize, GradeLevel? grade, SubjectType? subject, DifficultyLevel? difficulty, string? categoryTag, string? searchTerm)
    {
        var (items, totalCount) = await _repository.SearchAsync(page, pageSize, grade, subject, difficulty, categoryTag, searchTerm);
        return (_mapper.Map<IEnumerable<WheelQuestionResponseDto>>(items), totalCount);
    }

    public async Task<WheelQuestionResponseDto> GetByIdAsync(long id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null || item.IsDeleted) throw new KeyNotFoundException("Question not found");
        return _mapper.Map<WheelQuestionResponseDto>(item);
    }

    public async Task<WheelQuestionResponseDto> CreateAsync(CreateWheelQuestionDto dto, string userId)
    {
        var entity = _mapper.Map<WheelQuestion>(dto);
        entity.CreatedBy = userId;
        
        // Auto-calculate points if not provided
        if (entity.PointsValue == 0)
        {
            entity.PointsValue = dto.DifficultyLevel switch
            {
                DifficultyLevel.Easy => 10,
                DifficultyLevel.Medium => 20,
                DifficultyLevel.Hard => 30,
                _ => 10
            };
        }

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<WheelQuestionResponseDto>(entity);
    }

    public async Task<WheelQuestionResponseDto> UpdateAsync(long id, UpdateWheelQuestionDto dto, string userId)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted) throw new KeyNotFoundException("Question not found");

        _mapper.Map(dto, entity);
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<WheelQuestionResponseDto>(entity);
    }

    public async Task DeleteAsync(long id, string userId)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted) throw new KeyNotFoundException("Question not found");

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task BulkImportAsync(List<CreateWheelQuestionDto> dtos, string userId)
    {
        var entities = _mapper.Map<List<WheelQuestion>>(dtos);
        foreach (var e in entities)
        {
            e.CreatedBy = userId;
            e.CreatedDate = DateTime.Now;
            if (e.PointsValue == 0)
            {
                e.PointsValue = e.DifficultyLevel == DifficultyLevel.Hard ? 30 :
                                e.DifficultyLevel == DifficultyLevel.Medium ? 20 : 10;
            }
        }
        await _repository.BulkCreateAsync(entities);
    }

    public async Task<int> GetQuestionCountAsync(GradeLevel grade, SubjectType subject)
    {
        return await _repository.GetQuestionCountByGradeSubjectAsync(grade, subject);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync(GradeLevel? grade, SubjectType? subject)
    {
        return await _repository.GetCategoriesAsync(grade, subject);
    }
}
