
using Nafes.API.DTOs.MatchingGame;
using Nafes.API.Modules;
using Nafes.API.Repositories;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Nafes.API.Services;

public interface IMatchingQuestionService
{
    Task<IEnumerable<MatchingQuestionDto>> GetByGradeAndSubjectAsync(GradeLevel grade, SubjectType subject);
    Task<(IEnumerable<MatchingQuestionDto> Items, int TotalCount)> SearchAsync(int page, int pageSize, GradeLevel? grade, SubjectType? subject, string? searchTerm);
    Task<MatchingQuestionDto> GetByIdAsync(long id);
    Task<MatchingQuestionDto> CreateAsync(CreateMatchingQuestionDto dto, string createdBy);
    Task<MatchingQuestionDto> UpdateAsync(long id, UpdateMatchingQuestionDto dto, string updatedBy);
    Task DeleteAsync(long id, string deletedBy);
    Task BulkImportAsync(List<CreateMatchingQuestionDto> dtos, string createdBy);
}

public class MatchingQuestionService : IMatchingQuestionService
{
    private readonly IMatchingQuestionRepository _repository;

    public MatchingQuestionService(IMatchingQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MatchingQuestionDto>> GetByGradeAndSubjectAsync(GradeLevel grade, SubjectType subject)
    {
        var entities = await _repository.GetByGradeAndSubjectAsync(grade, subject);
        return entities.Select(MapToDto);
    }

    public async Task<(IEnumerable<MatchingQuestionDto> Items, int TotalCount)> SearchAsync(int page, int pageSize, GradeLevel? grade, SubjectType? subject, string? searchTerm)
    {
        var (items, totalCount) = await _repository.SearchAsync(page, pageSize, grade, subject, searchTerm);
        return (items.Select(MapToDto), totalCount);
    }

    public async Task<MatchingQuestionDto> GetByIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted)
            throw new KeyNotFoundException($"Matching question with ID {id} not found.");

        return MapToDto(entity);
    }

    public async Task<MatchingQuestionDto> CreateAsync(CreateMatchingQuestionDto dto, string createdBy)
    {
        var entity = new MatchingQuestion
        {
            GradeId = dto.GradeId,
            SubjectId = dto.SubjectId,
            LeftItemText = dto.LeftItemText,
            RightItemText = dto.RightItemText,
            DistractorItems = JsonSerializer.Serialize(dto.DistractorItems),
            DifficultyLevel = dto.DifficultyLevel,
            DisplayOrder = dto.DisplayOrder,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<MatchingQuestionDto> UpdateAsync(long id, UpdateMatchingQuestionDto dto, string updatedBy)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted)
            throw new KeyNotFoundException($"Matching question with ID {id} not found.");

        entity.GradeId = dto.GradeId;
        entity.SubjectId = dto.SubjectId;
        entity.LeftItemText = dto.LeftItemText;
        entity.RightItemText = dto.RightItemText;
        entity.DistractorItems = JsonSerializer.Serialize(dto.DistractorItems);
        entity.DifficultyLevel = dto.DifficultyLevel;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedDate = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task DeleteAsync(long id, string deletedBy)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return; // Already deleted or doesn't exist

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = deletedBy; // Track who deleted it

        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task BulkImportAsync(List<CreateMatchingQuestionDto> dtos, string createdBy)
    {
        var entities = dtos.Select(dto => new MatchingQuestion
        {
            GradeId = dto.GradeId,
            SubjectId = dto.SubjectId,
            LeftItemText = dto.LeftItemText,
            RightItemText = dto.RightItemText,
            DistractorItems = JsonSerializer.Serialize(dto.DistractorItems),
            DifficultyLevel = dto.DifficultyLevel,
            DisplayOrder = dto.DisplayOrder,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        }).ToList(); // Materialize before passing to repo if needed, handling large lists appropriately in real apps

        await _repository.BulkCreateAsync(entities);
    }

    private MatchingQuestionDto MapToDto(MatchingQuestion entity)
    {
        return new MatchingQuestionDto
        {
            Id = entity.Id,
            GradeId = entity.GradeId,
            SubjectId = entity.SubjectId,
            LeftItemText = entity.LeftItemText,
            RightItemText = entity.RightItemText,
            DistractorItems = JsonSerializer.Deserialize<List<string>>(entity.DistractorItems) ?? new List<string>(),
            DifficultyLevel = entity.DifficultyLevel,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive,
            CreatedDate = entity.CreatedDate,
            CreatedBy = entity.CreatedBy
        };
    }
}
